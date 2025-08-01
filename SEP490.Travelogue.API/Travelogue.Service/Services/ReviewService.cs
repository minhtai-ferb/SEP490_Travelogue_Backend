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
    Task<ReviewResponseDto> CreateTourReviewAsync(CreateTourReviewRequestDto dto, CancellationToken cancellationToken);
    Task<ReviewResponseDto> CreateWorkshopReviewAsync(CreateWorkshopReviewRequestDto dto, CancellationToken cancellationToken);
    Task<ReviewResponseDto> CreateTourGuideReviewAsync(CreateTourGuideReviewRequestDto dto, CancellationToken cancellationToken);
    Task<List<ReviewResponseDto>> GetReviewsByTourIdAsync(Guid tourId, CancellationToken cancellationToken);
    Task<List<ReviewResponseDto>> GetReviewsByWorkshopIdAsync(Guid workshopId, CancellationToken cancellationToken);
    Task<List<ReviewResponseDto>> GetReviewsByTourGuideIdAsync(Guid tourGuideId, CancellationToken cancellationToken);
    Task<bool> DeleteReviewAsync(Guid reviewId, Guid userId, CancellationToken cancellationToken);
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

    public async Task<ReviewResponseDto> CreateTourReviewAsync(CreateTourReviewRequestDto dto, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());

            var booking = await _unitOfWork.BookingRepository.ActiveEntities
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == dto.BookingId && b.UserId == currentUserId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Booking not found or you are not authorized.");

            var tour = await _unitOfWork.TourRepository.ActiveEntities
                .FirstOrDefaultAsync(t => t.Id == dto.TourId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour");

            var existingReview = await _unitOfWork.ReviewRepository.ActiveEntities
                .FirstOrDefaultAsync(r => r.BookingId == dto.BookingId, cancellationToken);
            if (existingReview != null)
            {
                throw CustomExceptionFactory.CreateBadRequestError("A review for this booking already exists.");
            }

            var review = new Review
            {
                UserId = currentUserId,
                BookingId = dto.BookingId,
                TourId = dto.TourId,
                Comment = dto.Comment,
                Rating = dto.Rating
            };

            await _unitOfWork.ReviewRepository.AddAsync(review);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return new ReviewResponseDto
            {
                Id = review.Id,
                UserId = review.UserId,
                UserName = booking.User?.FullName ?? string.Empty,
                BookingId = review.BookingId,
                TourId = review.TourId,
                WorkshopId = review.WorkshopId,
                TourGuideId = review.TourGuideId,
                Comment = review.Comment,
                Rating = review.Rating,
                CreatedAt = review.CreatedTime,
                UpdatedAt = review.LastUpdatedTime
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

    public async Task<ReviewResponseDto> CreateWorkshopReviewAsync(CreateWorkshopReviewRequestDto dto, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());

            var booking = await _unitOfWork.BookingRepository.ActiveEntities
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == dto.BookingId && b.UserId == currentUserId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Booking not found or you are not authorized.");

            var workshop = await _unitOfWork.WorkshopRepository.ActiveEntities
                .FirstOrDefaultAsync(t => t.Id == dto.WorkshopId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("workshop");

            var existingReview = await _unitOfWork.ReviewRepository.ActiveEntities
                .FirstOrDefaultAsync(r => r.BookingId == dto.BookingId, cancellationToken);
            if (existingReview != null)
            {
                throw CustomExceptionFactory.CreateBadRequestError("A review for this booking already exists.");
            }

            var review = new Review
            {
                UserId = currentUserId,
                BookingId = dto.BookingId,
                WorkshopId = dto.WorkshopId,
                Comment = dto.Comment,
                Rating = dto.Rating
            };

            await _unitOfWork.ReviewRepository.AddAsync(review);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return new ReviewResponseDto
            {
                Id = review.Id,
                UserId = review.UserId,
                UserName = booking.User?.FullName ?? string.Empty,
                BookingId = review.BookingId,
                TourId = review.TourId,
                WorkshopId = review.WorkshopId,
                TourGuideId = review.TourGuideId,
                Comment = review.Comment,
                Rating = review.Rating,
                CreatedAt = review.CreatedTime,
                UpdatedAt = review.LastUpdatedTime
            };
        }
        catch (CustomException)
        {
            await _unitOfWork.RollBackAsync();
            throw;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollBackAsync();
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<ReviewResponseDto> CreateTourGuideReviewAsync(CreateTourGuideReviewRequestDto dto, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());

            var booking = await _unitOfWork.BookingRepository.ActiveEntities
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == dto.BookingId && b.UserId == currentUserId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Booking not found or you are not authorized.");

            var tourGuide = await _unitOfWork.TourGuideRepository.ActiveEntities
                .FirstOrDefaultAsync(t => t.Id == dto.TourGuideId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour guide");

            var existingReview = await _unitOfWork.ReviewRepository.ActiveEntities
                .FirstOrDefaultAsync(r => r.BookingId == dto.BookingId, cancellationToken);
            if (existingReview != null)
            {
                throw CustomExceptionFactory.CreateBadRequestError("A review for this booking already exists.");
            }

            var review = new Review
            {
                UserId = currentUserId,
                BookingId = dto.BookingId,
                TourGuideId = dto.TourGuideId,
                Comment = dto.Comment,
                Rating = dto.Rating
            };

            await _unitOfWork.ReviewRepository.AddAsync(review);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return new ReviewResponseDto
            {
                Id = review.Id,
                UserId = review.UserId,
                UserName = booking.User?.FullName ?? string.Empty,
                BookingId = review.BookingId,
                TourId = review.TourId,
                WorkshopId = review.WorkshopId,
                TourGuideId = review.TourGuideId,
                Comment = review.Comment,
                Rating = review.Rating,
                CreatedAt = review.CreatedTime,
                UpdatedAt = review.LastUpdatedTime
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

    // public async Task<ReviewResponseDto> CreateReviewAsync(CreateReviewRequestDto dto, CancellationToken cancellationToken)
    // {
    //     var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());

    //     var targetCount = new[] { dto.TourId.HasValue, dto.WorkshopId.HasValue, dto.TourGuideId.HasValue }.Count(b => b);
    //     if (targetCount != 1)
    //     {
    //         throw CustomExceptionFactory.CreateBadRequestError("Exactly one of TourId, WorkshopId, or TourGuideId must be provided.");
    //     }

    //     var booking = await _unitOfWork.BookingRepository.ActiveEntities
    //         .Include(b => b.User)
    //         .FirstOrDefaultAsync(b => b.Id == dto.BookingId && b.UserId == currentUserId, cancellationToken)
    //         ?? throw CustomExceptionFactory.CreateNotFoundError("Booking not found or you are not authorized.");

    //     if (dto.TourId.HasValue)
    //     {
    //         var tourGuide = await _unitOfWork.TourRepository.ActiveEntities
    //             .FirstOrDefaultAsync(t => t.Id == dto.TourId.Value, cancellationToken)
    //             ?? throw CustomExceptionFactory.CreateNotFoundError("Tour not found.");
    //     }
    //     else if (dto.WorkshopId.HasValue)
    //     {
    //         var workshop = await _unitOfWork.WorkshopRepository.ActiveEntities
    //             .FirstOrDefaultAsync(w => w.Id == dto.WorkshopId.Value, cancellationToken)
    //             ?? throw CustomExceptionFactory.CreateNotFoundError("Workshop not found.");
    //     }
    //     else if (dto.TourGuideId.HasValue)
    //     {
    //         var tourGuide = await _unitOfWork.TourGuideRepository.ActiveEntities
    //             .FirstOrDefaultAsync(tg => tg.Id == dto.TourGuideId.Value, cancellationToken)
    //             ?? throw CustomExceptionFactory.CreateNotFoundError("Tour guide not found.");
    //     }

    //     var existingReview = await _unitOfWork.ReviewRepository.ActiveEntities
    //         .FirstOrDefaultAsync(r => r.BookingId == dto.BookingId, cancellationToken);
    //     if (existingReview != null)
    //     {
    //         throw CustomExceptionFactory.CreateBadRequestError("A review for this booking already exists.");
    //     }

    //     var review = new Review
    //     {
    //         UserId = currentUserId,
    //         BookingId = dto.BookingId,
    //         TourId = dto.TourId,
    //         WorkshopId = dto.WorkshopId,
    //         TourGuideId = dto.TourGuideId,
    //         Comment = dto.Comment,
    //         Rating = dto.Rating
    //     };

    //     await _unitOfWork.ReviewRepository.AddAsync(review);
    //     await _unitOfWork.SaveAsync();

    //     return new ReviewResponseDto
    //     {
    //         Id = review.Id,
    //         UserId = review.UserId,
    //         UserName = booking.User?.FullName ?? string.Empty,
    //         BookingId = review.BookingId,
    //         TourId = review.TourId,
    //         WorkshopId = review.WorkshopId,
    //         TourGuideId = review.TourGuideId,
    //         Comment = review.Comment,
    //         Rating = review.Rating,
    //         CreatedAt = review.CreatedTime,
    //         UpdatedAt = review.LastUpdatedTime
    //     };
    // }

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

            if (existingBooking.Status != BookingStatus.Completed)
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

    public async Task<List<ReviewResponseDto>> GetReviewsByTourIdAsync(Guid tourId, CancellationToken cancellationToken)
    {
        try
        {
            var reviews = await _unitOfWork.ReviewRepository.ActiveEntities
                .Include(r => r.User)
                .Where(r => r.TourId == tourId)
                .ToListAsync(cancellationToken);

            return reviews.Select(r => new ReviewResponseDto
            {
                Id = r.Id,
                UserId = r.UserId,
                UserName = r.User?.FullName ?? string.Empty,
                BookingId = r.BookingId,
                TourId = r.TourId,
                WorkshopId = r.WorkshopId,
                TourGuideId = r.TourGuideId,
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

    public async Task<List<ReviewResponseDto>> GetReviewsByWorkshopIdAsync(Guid workshopId, CancellationToken cancellationToken)
    {
        var reviews = await _unitOfWork.ReviewRepository.ActiveEntities
            .Include(r => r.User)
            .Where(r => r.WorkshopId == workshopId)
            .ToListAsync(cancellationToken);

        return reviews.Select(r => new ReviewResponseDto
        {
            Id = r.Id,
            UserId = r.UserId,
            UserName = r.User?.FullName ?? string.Empty,
            BookingId = r.BookingId,
            TourId = r.TourId,
            WorkshopId = r.WorkshopId,
            TourGuideId = r.TourGuideId,
            Comment = r.Comment,
            Rating = r.Rating,
            CreatedAt = r.CreatedTime,
            UpdatedAt = r.LastUpdatedTime
        }).ToList();
    }

    public async Task<List<ReviewResponseDto>> GetReviewsByTourGuideIdAsync(Guid tourGuideId, CancellationToken cancellationToken)
    {
        var reviews = await _unitOfWork.ReviewRepository.ActiveEntities
            .Include(r => r.User)
            .Where(r => r.TourGuideId == tourGuideId)
            .ToListAsync(cancellationToken);

        return reviews.Select(r => new ReviewResponseDto
        {
            Id = r.Id,
            UserId = r.UserId,
            UserName = r.User?.FullName ?? string.Empty,
            BookingId = r.BookingId,
            TourId = r.TourId,
            WorkshopId = r.WorkshopId,
            TourGuideId = r.TourGuideId,
            Comment = r.Comment,
            Rating = r.Rating,
            CreatedAt = r.CreatedTime,
            UpdatedAt = r.LastUpdatedTime
        }).ToList();
    }

    public async Task<bool> DeleteReviewAsync(Guid reviewId, Guid userId, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
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