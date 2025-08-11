

using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
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
    Task<TopTourResponseDto> GetTopToursByMonthAsync(int month, int year, int topCount, BookingStatus? status);
    Task<RevenueStatisticResponse> GetRevenueStatisticsAsync(DateTime fromDate, DateTime toDate);
    Task<BookingStatisticResponse> GetBookingStatisticsAsync(DateTime fromDate, DateTime toDate);
    Task<PagedResult<TourBookingItem>> GetTourBookingsAsync(
        Guid tourId,
        BookingStatus? status,
        int pageNumber,
        int pageSize);

    Task<PagedResult<TourBookingItem>> GetTourScheduleBookingsAsync(
        Guid tourScheduleId,
        BookingStatus? status,
        int pageNumber,
        int pageSize);

    Task<PagedResult<TourGuideBookingItem>> GetTourGuideBookingsAsync(
        Guid tourGuideId,
        BookingStatus? status,
        int pageNumber,
        int pageSize);
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

    public async Task<TopTourResponseDto> GetTopToursByMonthAsync(int month, int year, int topCount, BookingStatus? status)
    {
        try
        {
            var startDate = new DateTimeOffset(new DateTime(year, month, 1));
            var endDate = startDate.AddMonths(1);

            var filteredBookings = _unitOfWork.BookingRepository
                .ActiveEntities
                .Where(b => b.BookingType == BookingType.Tour
                    && b.TourId.HasValue
                    && b.BookingDate >= startDate
                    && b.BookingDate < endDate
                    && b.Status == BookingStatus.Confirmed);

            if (status.HasValue)
            {
                filteredBookings = filteredBookings.Where(b => b.Status == status.Value);
            }

            var groupedBookings = filteredBookings
                .GroupBy(b => new { b.TourId, b.Tour!.Name });

            var topTourItems = groupedBookings
                .Select(g => new TopTourItem
                {
                    TourId = g.Key.TourId!.Value,
                    TourName = g.Key.Name,
                    BookingCount = g.Count()
                });

            var orderedTopTours = topTourItems
                .OrderByDescending(t => t.BookingCount)
                .Take(topCount);

            var topTours = await orderedTopTours.ToListAsync();

            return new TopTourResponseDto
            {
                Month = month,
                Year = year,
                TopTours = topTours
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

    public async Task<RevenueStatisticResponse> GetRevenueStatisticsAsync(DateTime fromDate, DateTime toDate)
    {
        try
        {
            var adjustedToDate = toDate.Date.AddDays(1).AddTicks(-1);
            var adjustedFromDate = fromDate.Date;

            var filteredBookings = _unitOfWork.BookingRepository
                .ActiveEntities
                .Where(b => b.BookingDate >= adjustedFromDate
                    && b.BookingDate <= adjustedToDate
                    && b.Status == BookingStatus.Confirmed);

            var groupedByDate = filteredBookings
                .GroupBy(b => b.BookingDate.Date);

            var revenuePerDate = groupedByDate
                .Select(g => new RevenueDataItem
                {
                    Date = g.Key,
                    Revenue = g.Sum(b => b.FinalPrice)
                });

            var revenueData = await revenuePerDate.OrderBy(r => r.Date).ToListAsync();

            var totalRevenue = revenueData.Sum(r => r.Revenue);

            var allDates = Enumerable.Range(0, (toDate.Date - fromDate.Date).Days + 1)
                .Select(d => fromDate.Date.AddDays(d))
                .ToList();

            var completeRevenueData = allDates
                .GroupJoin(revenueData,
                    date => date,
                    data => data.Date,
                    (date, data) => new RevenueDataItem
                    {
                        Date = date,
                        Revenue = data.FirstOrDefault()?.Revenue ?? 0m
                    })
                .OrderBy(r => r.Date)
                .ToList();

            return new RevenueStatisticResponse
            {
                FromDate = fromDate,
                ToDate = toDate,
                TotalRevenue = totalRevenue,
                RevenueDataItem = completeRevenueData
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

    public async Task<BookingStatisticResponse> GetBookingStatisticsAsync(DateTime fromDate, DateTime toDate)
    {
        try
        {
            var adjustedFromDate = fromDate.Date;
            var adjustedToDate = toDate.Date.AddDays(1).AddTicks(-1);

            var bookingData = await _unitOfWork.BookingRepository
                .ActiveEntities
                .Where(b => b.BookingDate >= adjustedFromDate
                    && b.BookingDate <= adjustedToDate
                    && b.Status == BookingStatus.Confirmed)
                .GroupBy(b => b.BookingDate.Date)
                .Select(g => new
                {
                    Day = g.Key,
                    ScheduleCount = g.Count(b => b.TourScheduleId.HasValue && b.BookingType == BookingType.Tour),
                    TourGuideCount = g.Count(b => b.TourGuideId.HasValue && b.BookingType == BookingType.TourGuide),
                    TripPlanCount = g.Count(b => b.TripPlanId.HasValue && b.BookingType == BookingType.TourGuide)
                })
                .OrderBy(r => r.Day)
                .ToListAsync();

            var allDates = Enumerable.Range(0, (toDate.Date - fromDate.Date).Days + 1)
                .Select(d => fromDate.Date.AddDays(d))
                .ToList();

            var completeBookingData = allDates
                .GroupJoin(bookingData,
                    date => date,
                    data => data.Day,
                    (date, data) => new BookingStatisticItem
                    {
                        Day = date,
                        BookingSchedule = data.FirstOrDefault()?.ScheduleCount ?? 0,
                        BookingTourGuide = data.FirstOrDefault()?.TourGuideCount ?? 0,
                        BookingTripPlan = data.FirstOrDefault()?.TripPlanCount ?? 0
                    })
                .OrderBy(r => r.Day)
                .ToList();

            return new BookingStatisticResponse
            {
                FromDate = fromDate,
                ToDate = toDate,
                Data = completeBookingData
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

    public async Task<PagedResult<TourBookingItem>> GetTourBookingsAsync(
        Guid tourId,
        BookingStatus? status,
        int pageNumber,
        int pageSize)
    {
        try
        {
            bool tourExists = await _unitOfWork.TourRepository
                .ActiveEntities
                .AnyAsync(t => t.Id == tourId);

            if (!tourExists)
                throw CustomExceptionFactory.CreateNotFoundError("Tour");

            var query = _unitOfWork.TourScheduleRepository
                .ActiveEntities
                .Where(s => s.TourId == tourId)
                .SelectMany(s => s.Bookings
                    .Where(b => status == null || b.Status == status)
                    .Select(b => new
                    {
                        b.Id,
                        ScheduleId = s.Id,
                        UserName = b.User.FullName,
                        b.Status,
                        b.BookingType,
                        b.FinalPrice,
                        b.BookingDate
                    })
                );

            var totalRecords = await query.CountAsync();

            var bookingsData = await query
                .OrderByDescending(b => b.BookingDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var bookings = bookingsData.Select(b => new TourBookingItem
            {
                BookingId = b.Id,
                ScheduleId = b.ScheduleId,
                UserName = b.UserName,
                Status = b.Status,
                StatusText = _enumService.GetEnumDisplayName<BookingStatus>(b.Status) ?? string.Empty,
                BookingType = b.BookingType,
                BookingTypeText = _enumService.GetEnumDisplayName<BookingType>(b.BookingType) ?? string.Empty,
                FinalPrice = b.FinalPrice,
                BookingDate = b.BookingDate
            }).ToList();

            return new PagedResult<TourBookingItem>
            {
                Items = bookings,
                TotalCount = totalRecords,
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

    public async Task<PagedResult<TourBookingItem>> GetTourScheduleBookingsAsync(
    Guid tourScheduleId,
    BookingStatus? status,
    int pageNumber,
    int pageSize)
    {
        try
        {
            var schedule = await _unitOfWork.TourScheduleRepository
                .ActiveEntities
                .Where(s => s.Id == tourScheduleId)
                .Select(s => new
                {
                    s.Id,
                    s.DepartureDate,
                    s.MaxParticipant,
                    s.Status,
                    s.Reason,
                    s.AdultPrice,
                    s.ChildrenPrice,
                    TourName = s.Tour.Name,
                    s.TourId
                })
                .FirstOrDefaultAsync();

            if (schedule == null)
                throw CustomExceptionFactory.CreateNotFoundError("TourSchedule");

            var bookingsData = await _unitOfWork.BookingRepository
                .ActiveEntities
                .Where(b => b.TourScheduleId == tourScheduleId)
                .Where(b => status == null || b.Status == status)
                .OrderByDescending(b => b.BookingDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new
                {
                    b.Id,
                    b.User.FullName,
                    b.Status,
                    b.BookingType,
                    b.FinalPrice,
                    b.BookingDate
                })
                .ToListAsync();

            var totalBookings = await _unitOfWork.BookingRepository
                .ActiveEntities
                .Where(b => b.TourScheduleId == tourScheduleId)
                .Where(b => status == null || b.Status == status)
                .CountAsync();

            var bookings = bookingsData.Select(b => new TourBookingItem
            {
                BookingId = b.Id,
                UserName = b.FullName,
                Status = b.Status,
                StatusText = _enumService.GetEnumDisplayName<BookingStatus>(b.Status) ?? string.Empty,
                BookingType = b.BookingType,
                BookingTypeText = _enumService.GetEnumDisplayName<BookingType>(b.BookingType) ?? string.Empty,
                FinalPrice = b.FinalPrice
            }).ToList();

            return new PagedResult<TourBookingItem>
            {
                Items = bookings,
                TotalCount = totalBookings,
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

    public async Task<PagedResult<TourGuideBookingItem>> GetTourGuideBookingsAsync(
        Guid tourGuideId,
        BookingStatus? status,
        int pageNumber,
        int pageSize)
    {
        try
        {
            var tourGuide = await _unitOfWork.TourGuideRepository
                .ActiveEntities
                .Include(tg => tg.User)
                .Where(tg => tg.Id == tourGuideId)
                .Select(tg => new
                {
                    tg.Id,
                    tg.Price,
                    tg.Introduction,
                    tg.User.Email,
                    tg.User.FullName,
                    tg.User.Sex,
                    tg.User.Address,
                    tg.User.AvatarUrl
                })
                .FirstOrDefaultAsync();

            if (tourGuide == null)
                throw CustomExceptionFactory.CreateNotFoundError("TourGuide");

            var bookingsQuery = _unitOfWork.BookingRepository
                .ActiveEntities
                .Where(b => b.TourGuideId == tourGuideId)
                .Where(b => b.Status != BookingStatus.Cancelled)
                .Where(b => status == null || b.Status == status)
                .Select(b => new TourGuideBookingItem
                {
                    BookingId = b.Id,
                    TourName = b.Tour != null ? b.Tour.Name : "",
                    CustomerName = b.User.FullName,
                    Status = b.Status,
                    StatusText = _enumService.GetEnumDisplayName<BookingStatus>(b.Status) ?? string.Empty,
                    BookingType = b.BookingType,
                    BookingTypeText = _enumService.GetEnumDisplayName<BookingType>(b.BookingType) ?? string.Empty,
                    FinalPrice = b.FinalPrice
                });

            var totalBookings = await bookingsQuery.CountAsync();

            var bookings = await bookingsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<TourGuideBookingItem>
            {
                Items = bookings,
                TotalCount = totalBookings,
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
