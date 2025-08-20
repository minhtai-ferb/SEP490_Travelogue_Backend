using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Const;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.ReportModels;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface IReportService
{
    Task<ReportResponseDto> CreateReportAsync(CreateReportRequestDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteReportAsync(Guid reportId, CancellationToken cancellationToken);
    Task<List<ReportResponseDto>> GetMyReportsAsync(CancellationToken cancellationToken = default);
    Task<ReviewReportDetailDto> GetReviewWithReportByIdAsync(Guid reviewId, CancellationToken cancellationToken);
    Task<ReportResponseDto> UpdateReportAsync(Guid reportId, UpdateReportRequestDto dto, CancellationToken cancellationToken);
    Task<ReportResponseDto> ProcessReportAsync(Guid reportId, ProcessReportRequestDto dto, CancellationToken cancellationToken);
    Task<PagedResult<ReviewReportDetailDto>> GetReportedReviewsAsync(ReportStatus? status, CancellationToken cancellationToken, int pageNumber = 1, int pageSize = 10);
}

public class ReportService : IReportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public ReportService(IUnitOfWork unitOfWork, IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task<ReportResponseDto> CreateReportAsync(CreateReportRequestDto dto, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());

            var review = await _unitOfWork.ReviewRepository.ActiveEntities
                .Include(r => r.Reports)
                .FirstOrDefaultAsync(r => r.Id == dto.ReviewId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Review");

            // Kiểm tra user đã report chưa
            var existingReport = review.Reports.FirstOrDefault(r => r.UserId == currentUserId);
            if (existingReport != null)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Bạn đã báo cáo đánh giá này trước đó");
            }

            // Tạo report mới
            var report = new Report
            {
                UserId = currentUserId,
                ReviewId = dto.ReviewId,
                Reason = dto.Reason,
                Status = ReportStatus.Pending,
                CreatedTime = DateTime.UtcNow
            };

            review.FinalReportStatus = ReportStatus.Pending;

            _unitOfWork.ReviewRepository.Update(review);
            await _unitOfWork.ReportRepository.AddAsync(report);

            review.FinalReportStatus = ReportStatus.Pending;
            _unitOfWork.ReviewRepository.Update(review);

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return new ReportResponseDto
            {
                Id = report.Id,
                UserId = report.UserId,
                ReviewId = report.ReviewId,
                Reason = report.Reason,
                Status = report.Status,
                CreatedTime = report.CreatedTime,
                LastUpdatedTime = report.LastUpdatedTime
            };
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync();
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<List<ReportResponseDto>> GetMyReportsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());

            var reports = await _unitOfWork.ReportRepository.ActiveEntities
                .Where(r => r.UserId == currentUserId)
                .Select(r => new ReportResponseDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    ReviewId = r.ReviewId,
                    Reason = r.Reason,
                    Status = r.Status,
                    CreatedTime = r.CreatedTime,
                    LastUpdatedTime = r.LastUpdatedTime
                })
                .ToListAsync(cancellationToken);

            return reports;
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<ReviewReportDetailDto> GetReviewWithReportByIdAsync(Guid reviewId, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            if (!_userContextService.HasAnyRole(AppRole.ADMIN, AppRole.MODERATOR))
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var review = await _unitOfWork.ReviewRepository.ActiveEntities
                                .Include(r => r.Reports)
                                .ThenInclude(rep => rep.User)
                                .FirstOrDefaultAsync(r => r.Id == reviewId, cancellationToken)
                                ?? throw CustomExceptionFactory.CreateNotFoundError("Review not found.");

            var reports = new ReviewReportDetailDto
            {
                ReviewId = review.Id,
                Comment = review.Comment,
                Rating = review.Rating,
                UserId = review.UserId,
                UserName = review.User.FullName,
                FinalReportStatus = review.FinalReportStatus,
                HandledBy = review.HandledBy,
                HandledAt = review.HandledAt,
                ModeratorNote = review.ModeratorNote,
                Reports = review.Reports.Select(r => new ReportResponseDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    ReviewId = r.ReviewId,
                    Reason = r.Reason,
                    Status = r.Status,
                    CreatedTime = r.CreatedTime,
                    LastUpdatedTime = r.LastUpdatedTime
                }).ToList()
            };

            return reports;
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<ReportResponseDto> ProcessReportAsync(Guid reportId, ProcessReportRequestDto dto, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            if (!_userContextService.HasAnyRole(AppRole.ADMIN, AppRole.MODERATOR))
                throw CustomExceptionFactory.CreateForbiddenError();

            var report = await _unitOfWork.ReportRepository.ActiveEntities
                .FirstOrDefaultAsync(r => r.Id == reportId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Report not found.");

            var review = await _unitOfWork.ReviewRepository.ActiveEntities
                .Include(r => r.Reports)
                .FirstOrDefaultAsync(r => r.Id == report.ReviewId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Review not found.");

            // Cập nhật trạng thái cho báo cáo hiện tại
            report.Status = dto.Status;
            report.LastUpdatedBy = currentUserId.ToString();
            report.LastUpdatedTime = DateTime.UtcNow;

            // Nếu admin duyệt báo cáo (Processed) → cập nhật FinalReportStatus của review
            if (dto.Status == ReportStatus.Processed)
            {
                review.FinalReportStatus = ReportStatus.Processed;
                review.HandledBy = currentUserId;
                review.HandledAt = DateTime.UtcNow;
                review.ModeratorNote = dto.Note;

                // Ẩn hoặc xóa review (tùy logic)
                review.IsActive = false;

                // Đóng tất cả các báo cáo khác về review này
                foreach (var r in review.Reports)
                {
                    if (r.Status == ReportStatus.Pending)
                        r.Status = ReportStatus.Processed;
                }
            }
            else if (dto.Status == ReportStatus.Rejected)
            {
                review.FinalReportStatus = ReportStatus.Rejected;
                review.HandledBy = currentUserId;
                review.HandledAt = DateTime.UtcNow;
                review.ModeratorNote = dto.Note;

                // Các báo cáo khác có thể cập nhật sang Rejected
                foreach (var r in review.Reports)
                {
                    if (r.Status == ReportStatus.Pending)
                        r.Status = ReportStatus.Rejected;
                }
            }

            _unitOfWork.ReviewRepository.Update(review);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return new ReportResponseDto
            {
                Id = report.Id,
                UserId = report.UserId,
                ReviewId = report.ReviewId,
                Reason = report.Reason,
                Status = report.Status,
                CreatedTime = report.CreatedTime,
                LastUpdatedTime = report.LastUpdatedTime
            };
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync();
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<ReportResponseDto> UpdateReportAsync(Guid reportId, UpdateReportRequestDto dto, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var report = await _unitOfWork.ReportRepository.ActiveEntities
                .FirstOrDefaultAsync(r => r.Id == reportId && r.UserId == Guid.Parse(currentUserId), cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Report not found or you are not authorized.");

            if (report.Status != ReportStatus.Pending)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Report đã được xử lý, không thể cập nhật");
            }

            report.Reason = dto.Reason;
            report.LastUpdatedTime = DateTime.UtcNow;
            report.LastUpdatedBy = currentUserId;

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return new ReportResponseDto
            {
                Id = report.Id,
                UserId = report.UserId,
                ReviewId = report.ReviewId,
                Reason = report.Reason,
                Status = report.Status,
                CreatedTime = report.CreatedTime,
                LastUpdatedTime = report.LastUpdatedTime
            };
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync();
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<bool> DeleteReportAsync(Guid reportId, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var report = await _unitOfWork.ReportRepository.ActiveEntities
                .FirstOrDefaultAsync(r => r.Id == reportId && r.UserId == Guid.Parse(currentUserId), cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Report");

            if (report.Status != ReportStatus.Pending)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Report đang được xử lý, không thể xóa");
            }

            report.IsDeleted = true;
            report.DeletedBy = currentUserId;
            report.DeletedTime = DateTime.UtcNow;

            _unitOfWork.ReportRepository.Update(report);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync();
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<PagedResult<ReviewReportDetailDto>> GetReportedReviewsAsync(ReportStatus? status, CancellationToken cancellationToken, int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            if (!_userContextService.HasAnyRole(AppRole.ADMIN, AppRole.MODERATOR))
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var query = _unitOfWork.ReviewRepository.ActiveEntities
                .Include(r => r.Reports)
                .Include(r => r.User)
                .Where(r => r.Reports.Any());

            if (status.HasValue)
            {
                query = query.Where(r => r.FinalReportStatus == status.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            if (totalCount == 0)
            {
                return new PagedResult<ReviewReportDetailDto>
                {
                    Items = new List<ReviewReportDetailDto>(),
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }

            var reviewReportItems = await query
                .OrderByDescending(r => r.CreatedTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var reviewReportResponses = reviewReportItems.Select(review => new ReviewReportDetailDto
            {
                ReviewId = review.Id,
                Comment = review.Comment,
                Rating = review.Rating,
                UserId = review.UserId,
                UserName = review.User.FullName,
                FinalReportStatus = review.FinalReportStatus,
                HandledBy = review.HandledBy,
                HandledAt = review.HandledAt,
                ModeratorNote = review.ModeratorNote,
                Reports = review.Reports.Select(r => new ReportResponseDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    ReviewId = r.ReviewId,
                    Reason = r.Reason,
                    Status = r.Status,
                    CreatedTime = r.CreatedTime,
                    LastUpdatedTime = r.LastUpdatedTime
                }).ToList()
            }).ToList();

            return new PagedResult<ReviewReportDetailDto>
            {
                Items = reviewReportResponses,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }
}