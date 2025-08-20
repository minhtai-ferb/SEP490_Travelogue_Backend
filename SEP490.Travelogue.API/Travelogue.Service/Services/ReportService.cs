using Microsoft.EntityFrameworkCore;
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
    Task<List<ReportResponseDto>> GetReportsByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ReportResponseDto> UpdateReportAsync(Guid reportId, UpdateReportRequestDto dto, CancellationToken cancellationToken);
    Task<List<ReportResponseDto>> GetAllReportsAsync(CancellationToken cancellationToken = default);
    Task<List<ReportResponseDto>> GetReportsByStatusAsync(ReportStatus status, CancellationToken cancellationToken = default);
    Task<ReportResponseDto> ProcessReportAsync(Guid reportId, ProcessReportRequestDto dto, CancellationToken cancellationToken);
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

            _unitOfWork.ReportRepository.Remove(report);
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

    public async Task<List<ReportResponseDto>> GetReportsByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            if (!_userContextService.HasAnyRole(AppRole.ADMIN, AppRole.MODERATOR))
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var reports = await _unitOfWork.ReportRepository.ActiveEntities
                .Where(r => r.ReviewId == id)
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
        catch (CustomException)
        {
            throw;
        }
        catch (Exception ex)
        {
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

    public async Task<List<ReportResponseDto>> GetAllReportsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            if (!_userContextService.HasAnyRole(AppRole.ADMIN, AppRole.MODERATOR))
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var reports = await _unitOfWork.ReportRepository.ActiveEntities
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
        catch (CustomException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<List<ReportResponseDto>> GetReportsByStatusAsync(ReportStatus status, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            if (!_userContextService.HasAnyRole(AppRole.ADMIN, AppRole.MODERATOR))
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var reports = await _unitOfWork.ReportRepository.ActiveEntities
                .Where(r => r.Status == status)
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
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var report = await _unitOfWork.ReportRepository.ActiveEntities
                .FirstOrDefaultAsync(r => r.Id == reportId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Report not found.");

            var review = await _unitOfWork.ReviewRepository.ActiveEntities
                .Include(r => r.Booking)
                .ThenInclude(b => b.Tour)
                .FirstOrDefaultAsync(r => r.Id == report.ReviewId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Review not found.");

            report.Status = dto.Status;
            report.LastUpdatedBy = currentUserId.ToString();
            report.LastUpdatedTime = DateTime.UtcNow;

            if (dto.Status == ReportStatus.Processed)
            {
                review.IsDeleted = true;
                _unitOfWork.ReviewRepository.Update(review);

                await _unitOfWork.SaveAsync();
            }

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
}