using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.ReviewModels;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface IReviewService
{
    Task<ReviewResponseDto> CreateReviewAsync(CreateReviewRequestDto dto, CancellationToken cancellationToken);
    Task<List<ReviewResponseDto>> GetMyReviewsAsync(int? rating = null, CancellationToken cancellationToken = default);
    Task<List<ReviewResponseDto>> GetReviewsByIdAsync(Guid id, CancellationToken cancellationToken);
}

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public ReviewService(IUnitOfWork unitOfWork, IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task<ReviewResponseDto> CreateReviewAsync(CreateReviewRequestDto dto, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());

            var existingBooking = await _unitOfWork.BookingRepository.ActiveEntities
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == dto.BookingId && b.UserId == currentUserId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Không tìm thấy booking hoặc bạn không có quyền truy cập.");

            if (existingBooking.Status != BookingStatus.Confirmed)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Chỉ có thể đánh giá các booking đã hoàn thành.");
            }

            var review = new Review();

            switch (existingBooking.BookingType)
            {
                case BookingType.Tour:
                    if (existingBooking.TourScheduleId == null)
                        throw CustomExceptionFactory.CreateBadRequestError("TourScheduleId không tồn tại trong booking tour.");

                    var tourSchedule = await _unitOfWork.TourScheduleRepository
                        .ActiveEntities
                        .FirstOrDefaultAsync(ts => ts.Id == existingBooking.TourScheduleId)
                        ?? throw CustomExceptionFactory.CreateNotFoundError("tour schedule");

                    // review.TourId = tourSchedule.TourId;
                    review.UserId = currentUserId;
                    review.BookingId = dto.BookingId;
                    review.Comment = dto.Comment;
                    review.Rating = dto.Rating;
                    break;

                case BookingType.Workshop:
                    if (existingBooking.WorkshopScheduleId == null)
                        throw CustomExceptionFactory.CreateBadRequestError("WorkshopScheduleId không tồn tại trong booking workshop.");

                    var workshopSchedule = await _unitOfWork.WorkshopScheduleRepository
                        .ActiveEntities
                        .FirstOrDefaultAsync(ws => ws.Id == existingBooking.WorkshopScheduleId)
                        ?? throw CustomExceptionFactory.CreateNotFoundError("workshop schedule");

                    review.UserId = currentUserId;
                    review.BookingId = dto.BookingId;
                    review.Comment = dto.Comment;
                    review.Rating = dto.Rating;
                    break;

                case BookingType.TourGuide:
                    if (existingBooking.TourGuideId == null)
                        throw CustomExceptionFactory.CreateBadRequestError("TourGuideId không tồn tại trong booking tour guide.");
                    var tourGuide = await _unitOfWork.TourGuideRepository
                        .ActiveEntities
                        .Include(tg => tg.TourGuideSchedules)
                        .FirstOrDefaultAsync(tg => tg.Id == existingBooking.TourGuideId)
                        ?? throw CustomExceptionFactory.CreateNotFoundError("tour guide");

                    review.UserId = currentUserId;
                    review.BookingId = dto.BookingId;
                    review.Comment = dto.Comment;
                    review.Rating = dto.Rating;
                    break;

                default:
                    throw CustomExceptionFactory.CreateBadRequestError("Loại booking không được hỗ trợ.");
            }

            var existingReview = await _unitOfWork.ReviewRepository.ActiveEntities
                .FirstOrDefaultAsync(r => r.BookingId == dto.BookingId, cancellationToken);
            if (existingReview != null)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Booking này đã được đánh giá.");
            }

            await _unitOfWork.ReviewRepository.AddAsync(review);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return new ReviewResponseDto
            {
                Id = review.Id,
                UserId = review.UserId,
                UserName = existingBooking.User?.FullName ?? string.Empty,
                BookingId = review.BookingId,
                Comment = review.Comment,
                Rating = review.Rating,
                CreatedAt = review.CreatedTime,
                UpdatedAt = review.LastUpdatedTime
            };
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<List<ReviewResponseDto>> GetMyReviewsAsync(int? rating = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());

            if (string.IsNullOrWhiteSpace(currentUserId.ToString()))
                throw CustomExceptionFactory.CreateForbiddenError();

            var query = _unitOfWork.ReviewRepository.ActiveEntities
                .Include(r => r.User)
                .Include(r => r.Booking)
                .Where(r => r.UserId == currentUserId);

            if (rating.HasValue)
            {
                query = query.Where(r => r.Rating == rating.Value);
            }

            var reviews = await query.ToListAsync(cancellationToken);

            return reviews.Select(r => new ReviewResponseDto
            {
                Id = r.Id,
                UserId = r.UserId,
                UserName = r.User?.FullName ?? string.Empty,
                BookingId = r.BookingId,
                TourId = r.Booking.TourId,
                WorkshopId = r.Booking.WorkshopId,
                TourGuideId = r.Booking.TourGuideId,
                Comment = r.Comment,
                Rating = r.Rating,
                CreatedAt = r.CreatedTime,
                UpdatedAt = r.LastUpdatedTime
            }).ToList();
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

    public async Task<List<ReviewResponseDto>> GetReviewsByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var reviews = await _unitOfWork.ReviewRepository.ActiveEntities
                .Include(r => r.User)
                .Include(r => r.Booking)
                .Where(r => r.Id == id)
                .ToListAsync(cancellationToken);

            return reviews.Select(r => new ReviewResponseDto
            {
                Id = r.Id,
                UserId = r.UserId,
                UserName = r.User?.FullName ?? string.Empty,
                BookingId = r.BookingId,
                TourId = r.Booking.TourId,
                WorkshopId = r.Booking.WorkshopId,
                TourGuideId = r.Booking.TourGuideId,
                Comment = r.Comment,
                Rating = r.Rating,
                CreatedAt = r.CreatedTime,
                UpdatedAt = r.LastUpdatedTime
            }).ToList();
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

    public async Task<bool> DeleteReviewAsync(Guid reviewId, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var userId = Guid.Parse(_userContextService.GetCurrentUserId());

            var review = await _unitOfWork.ReviewRepository.ActiveEntities
                .FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Review not found or you are not authorized.");

            review.IsDeleted = true;
            review.LastUpdatedTime = DateTimeOffset.UtcNow;

            await _unitOfWork.SaveAsync();
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
}