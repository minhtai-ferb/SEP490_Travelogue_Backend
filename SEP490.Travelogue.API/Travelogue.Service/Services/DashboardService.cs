

using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Const;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.BookingModels;
using Travelogue.Service.BusinessModels.DashboardModels;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface IDashboardService
{
    Task<BookingStatisticsDto> GetBookingStatisticsAsync(BookingFilterDto filter);
}

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEnumService _enumService;
    private readonly IUserContextService _userContextService;

    public DashboardService(
        IUnitOfWork unitOfWork,
        IEnumService enumService,
        IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _enumService = enumService;
        _userContextService = userContextService;
    }

    public async Task<BookingStatisticsDto> GetBookingStatisticsAsync(BookingFilterDto filter)
    {
        var isAllowed = _userContextService.HasAnyRole(AppRole.MODERATOR, AppRole.ADMIN);
        if (!isAllowed)
        {
            throw CustomExceptionFactory.CreateForbiddenError();
        }

        IQueryable<Booking> query = _unitOfWork.BookingRepository
            .ActiveEntities
            .Include(b => b.TourGuide)
                .ThenInclude(tg => tg != null ? tg.User : null)
            .Include(b => b.Tour)
            .Include(b => b.TourSchedule)
            .Include(b => b.TripPlan)
            .Include(b => b.Workshop)
            .Include(b => b.WorkshopSchedule)
            .Include(b => b.Promotion);

        if (filter.Status.HasValue)
            query = query.Where(b => b.Status == filter.Status.Value);

        // theo ngày
        if (filter.StartDate.HasValue && filter.EndDate.HasValue)
        {
            if (filter.StartDate > filter.EndDate)
                throw CustomExceptionFactory.CreateBadRequestError("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.");

            query = query.Where(b => b.BookingDate >= filter.StartDate.Value && b.BookingDate <= filter.EndDate.Value);
        }

        else if (filter.StartDate.HasValue)
            query = query.Where(b => b.BookingDate >= filter.StartDate.Value);
        else if (filter.EndDate.HasValue)
            query = query.Where(b => b.BookingDate <= filter.EndDate.Value);

        // thống kês
        var totalBookings = await query.CountAsync();
        var completedBookings = await query.CountAsync(b => b.Status == BookingStatus.Confirmed);
        var cancelledBookings = await query.CountAsync(b => b.Status == BookingStatus.Cancelled);
        var totalRevenue = await query.SumAsync(b => (decimal?)b.FinalPrice) ?? 0;
        var actualRevenue = await query.SumAsync(b => (decimal?)b.FinalPrice - (decimal?)b.DiscountAmount ?? 0);
        var cancelRate = totalBookings > 0 ? (double)cancelledBookings / totalBookings * 100 : 0;

        // Đếm theo status
        var statusCount = await query
            .GroupBy(b => b.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(
                x => _enumService.GetEnumDisplayName<BookingStatus>(x.Status),
                x => x.Count
            );

        // Top tour
        var topTours = await query
            .Where(b => b.TourId != null)
            .GroupBy(b => new { b.TourId, b.Tour.Name })
            .Select(g => new TopItemDto
            {
                Id = g.Key.TourId ?? Guid.Empty,
                Name = g.Key.Name,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToListAsync();

        // Top guide
        var topGuides = await query
            .Where(b => b.TourGuideId != null)
            .GroupBy(b => new { b.TourGuideId, Name = b.TourGuide.User.FullName })
            .Select(g => new TopItemDto
            {
                Id = g.Key.TourGuideId ?? Guid.Empty,
                Name = g.Key.Name,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToListAsync();

        // var bookings = await query
        //     .OrderByDescending(b => b.BookingDate)
        //     .Select(b => new BookingDataModel
        //     {
        //         Id = b.Id,
        //         UserId = b.UserId,
        //         TourId = b.TourId,
        //         TourScheduleId = b.TourScheduleId,
        //         TourGuideId = b.TourGuideId,
        //         TripPlanId = b.TripPlanId,
        //         WorkshopId = b.WorkshopId,
        //         WorkshopScheduleId = b.WorkshopScheduleId,
        //         PaymentLinkId = b.PaymentLinkId,
        //         Status = b.Status,
        //         StatusText = _enumService.GetEnumDisplayName<BookingStatus>(b.Status),
        //         BookingType = b.BookingType,
        //         BookingTypeText = _enumService.GetEnumDisplayName<BookingType>(b.BookingType),
        //         BookingDate = b.BookingDate,
        //         CancelledAt = b.CancelledAt,
        //         PromotionId = b.PromotionId,
        //         OriginalPrice = b.OriginalPrice,
        //         DiscountAmount = b.DiscountAmount,
        //         FinalPrice = b.FinalPrice
        //     })
        //     .ToListAsync();

        return new BookingStatisticsDto
        {
            // Bookings = bookings,
            TotalBookings = totalBookings,
            CompletedBookings = completedBookings,
            CancelledBookings = cancelledBookings,
            TotalRevenue = totalRevenue,
            ActualRevenue = actualRevenue,
            CancelRate = cancelRate,
            StatusCount = statusCount,
            TopTours = topTours,
            TopGuides = topGuides
        };
    }
}
