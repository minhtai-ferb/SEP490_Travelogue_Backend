

using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Const;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.BookingModels;
using Travelogue.Service.BusinessModels.DashboardModels;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface IDashboardService
{
    Task<TopTourResponseDto> GetTopToursByMonthAsync(int month, int year, int topCount, BookingStatus? status);
    // Task<RevenueStatisticResponse> GetSystemRevenueStatisticsAsync(DateTime fromDate, DateTime toDate);
    Task<RevenueStatisticDto> GetSystemRevenueStatisticsAsync(DateTime fromDate, DateTime toDate);
    Task<AdminRevenueDataDto> GetAdminRevenueStatisticsAsync(DateTime fromDate, DateTime toDate);
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

    Task<TourStatisticDto> GetTourBookingStatisticAsync(Guid tourId);
    Task<TourStatisticDto> GetTourGuideBookingStatisticAsync(Guid tourGuideId);
    Task<TourStatisticDto> GetWorkshopScheduleBookingStatisticAsync(Guid workshopScheduleId);
    Task<TourStatisticDto> GetWorkshopBookingStatisticAsync(Guid workshopId);
    Task<TourStatisticDto> GetTourScheduleBookingStatisticAsync(Guid tourScheduleId);
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

    // public async Task<RevenueStatisticResponse> GetRevenueStatisticsAsync(DateTime fromDate, DateTime toDate)
    // {
    //     try
    //     {
    //         var adjustedToDate = toDate.Date.AddDays(1).AddTicks(-1);
    //         var adjustedFromDate = fromDate.Date;

    //         var filteredBookings = _unitOfWork.BookingRepository
    //             .ActiveEntities
    //             .Where(b => b.BookingDate >= adjustedFromDate
    //                 && b.BookingDate <= adjustedToDate
    //                 && b.Status == BookingStatus.Confirmed);

    //         var groupedByDate = filteredBookings
    //             .GroupBy(b => b.BookingDate.Date);

    //         var revenuePerDate = groupedByDate
    //             .Select(g => new RevenueDataItem
    //             {
    //                 Date = g.Key,
    //                 Revenue = g.Sum(b => b.FinalPrice)
    //             });

    //         var revenueData = await revenuePerDate.OrderBy(r => r.Date).ToListAsync();

    //         var totalRevenue = revenueData.Sum(r => r.Revenue);

    //         var allDates = Enumerable.Range(0, (toDate.Date - fromDate.Date).Days + 1)
    //             .Select(d => fromDate.Date.AddDays(d))
    //             .ToList();

    //         var completeRevenueData = allDates
    //             .GroupJoin(revenueData,
    //                 date => date,
    //                 data => data.Date,
    //                 (date, data) => new RevenueDataItem
    //                 {
    //                     Date = date,
    //                     Revenue = data.FirstOrDefault()?.Revenue ?? 0m
    //                 })
    //             .OrderBy(r => r.Date)
    //             .ToList();

    //         return new RevenueStatisticResponse
    //         {
    //             FromDate = fromDate,
    //             ToDate = toDate,
    //             TotalRevenue = totalRevenue,
    //             RevenueDataItem = completeRevenueData
    //         };
    //     }
    //     catch (CustomException)
    //     {
    //         throw;
    //     }
    //     catch (Exception ex)
    //     {
    //         throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
    //     }
    // }

    // public async Task<RevenueStatisticResponse> GetRevenueStatisticsAsync(DateTime fromDate, DateTime toDate)
    // {
    //     try
    //     {
    //         var adjustedToDate = toDate.Date.AddDays(1).AddTicks(-1);
    //         var adjustedFromDate = fromDate.Date;

    //         var filteredBookings = _unitOfWork.BookingRepository
    //             .ActiveEntities
    //             .Where(b =>
    //                 b.StartDate >= adjustedFromDate &&
    //                 b.StartDate <= adjustedToDate &&
    //                 (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed)
    //             );

    //         var commissionRate = await GetBookingCommissionPercentAsync();

    //         var groupedByDate = filteredBookings
    //             .GroupBy(b => b.StartDate.Date)
    //             .Select(g => new RevenueDataItem
    //             {
    //                 Date = g.Key,
    //                 GrossRevenue = g.Sum(b => b.FinalPrice),
    //                 Commission = g.Sum(b =>
    //                     (b.BookingType == BookingType.TourGuide || b.BookingType == BookingType.Workshop)
    //                         ? b.FinalPrice * commissionRate
    //                         : 0m),
    //                 NetRevenue = g.Sum(b =>
    //                     b.BookingType == BookingType.Tour
    //                         ? b.FinalPrice
    //                         : b.FinalPrice * commissionRate)
    //             });

    //         var revenueData = await groupedByDate.OrderBy(r => r.Date).ToListAsync();

    //         var totalGrossRevenue = revenueData.Sum(r => r.GrossRevenue);
    //         var totalNetRevenue = revenueData.Sum(r => r.NetRevenue);
    //         var totalCommission = revenueData.Sum(r => r.Commission);

    //         var allDates = Enumerable.Range(0, (toDate.Date - fromDate.Date).Days + 1)
    //             .Select(d => fromDate.Date.AddDays(d))
    //             .ToList();

    //         var completeRevenueData = allDates
    //             .GroupJoin(revenueData,
    //                 date => date,
    //                 data => data.Date,
    //                 (date, data) => new RevenueDataItem
    //                 {
    //                     Date = date,
    //                     GrossRevenue = data.FirstOrDefault()?.GrossRevenue ?? 0m,
    //                     Commission = data.FirstOrDefault()?.Commission ?? 0m,
    //                     NetRevenue = data.FirstOrDefault()?.NetRevenue ?? 0m
    //                 })
    //             .OrderBy(r => r.Date)
    //             .ToList();

    //         return new RevenueStatisticResponse
    //         {
    //             FromDate = fromDate,
    //             ToDate = toDate,
    //             GrossRevenue = totalGrossRevenue,
    //             NetRevenue = totalNetRevenue,
    //             TotalRevenue = totalGrossRevenue,
    //             Commission = totalCommission,
    //             RevenueDataItem = completeRevenueData
    //         };
    //     }
    //     catch (CustomException)
    //     {
    //         throw;
    //     }
    //     catch (Exception ex)
    //     {
    //         throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
    //     }
    // }

    public async Task<RevenueStatisticDto> GetSystemRevenueStatisticsAsync(DateTime fromDate, DateTime toDate)
    {
        try
        {
            var adjustedToDate = toDate.Date.AddDays(1).AddTicks(-1);
            var adjustedFromDate = fromDate.Date;

            var result = new RevenueStatisticDto();

            var filteredBookings = _unitOfWork.BookingRepository
                .ActiveEntities
                .Where(b =>
                    b.StartDate >= adjustedFromDate &&
                    b.StartDate <= adjustedToDate
                // &&
                // (b.Status == BookingStatus.Completed)
                );

            // var commissionRate = (await GetBookingCommissionPercentAsync()) / 100;

            var groupedByDateGross = filteredBookings
                .Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed)
                .GroupBy(b => b.StartDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Tour = g.Where(b => b.BookingType == BookingType.Tour).Sum(b => b.FinalPrice),
                    BookingTourGuide = g.Where(b => b.BookingType == BookingType.TourGuide).Sum(b => b.FinalPrice),
                    BookingWorkshop = g.Where(b => b.BookingType == BookingType.Workshop).Sum(b => b.FinalPrice),
                });

            var grossRevenue = new RevenueDetailDto();
            var revenueData = await groupedByDateGross.OrderBy(r => r.Date).ToListAsync();

            var tourGrossRevenue = revenueData.Sum(r => r.Tour);
            var tourGuideGrossRevenue = revenueData.Sum(r => r.BookingTourGuide);
            var tourWorkshopRevenue = revenueData.Sum(r => r.BookingWorkshop);

            var revenueByCategoryDto = new RevenueByCategoryDto
            {
                Tour = tourGrossRevenue,
                BookingTourGuide = tourGuideGrossRevenue,
                BookingWorkshop = tourWorkshopRevenue,
            };

            var allDates = Enumerable.Range(0, (toDate.Date - fromDate.Date).Days + 1)
                .Select(d => fromDate.Date.AddDays(d))
                .ToList();

            var completeRevenueData = allDates
                .GroupJoin(revenueData,
                    date => date,
                    data => data.Date,
                    (date, data) => new DailyRevenueStatDto
                    {
                        Date = date,
                        Total = (data.FirstOrDefault()?.Tour ?? 0m)
                            + (data.FirstOrDefault()?.BookingTourGuide ?? 0m)
                            + (data.FirstOrDefault()?.BookingWorkshop ?? 0m),
                        Tour = data.FirstOrDefault()?.Tour ?? 0m,
                        BookingTourGuide = data.FirstOrDefault()?.BookingTourGuide ?? 0m,
                        BookingWorkshop = data.FirstOrDefault()?.BookingWorkshop ?? 0m
                    })
                .OrderBy(r => r.Date)
                .ToList();

            grossRevenue.Total = revenueByCategoryDto.Tour + revenueByCategoryDto.BookingTourGuide + revenueByCategoryDto.BookingWorkshop;
            grossRevenue.ByCategory = revenueByCategoryDto;
            grossRevenue.DailyStats = completeRevenueData;

            result.GrossRevenue = grossRevenue;

            // --------------------------------------------------

            var groupedByDateNet = filteredBookings
                .Where(b => b.Status == BookingStatus.Completed)
                .GroupBy(b => b.StartDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Tour = g.Where(b => b.BookingType == BookingType.Tour).Sum(b => b.FinalPrice),
                    BookingTourGuide = g.Where(b => b.BookingType == BookingType.TourGuide).Sum(b => b.FinalPrice),
                    BookingWorkshop = g.Where(b => b.BookingType == BookingType.Workshop).Sum(b => b.FinalPrice),

                    /* 
                    // Commission = g.Sum(b =>
                    //     (b.BookingType == BookingType.TourGuide || b.BookingType == BookingType.Workshop)
                    //         ? b.FinalPrice * commissionRate
                    //         : 0m),
                    // NetRevenue = g.Sum(b =>
                    //     b.BookingType == BookingType.Tour
                    //         ? b.FinalPrice
                    //         : b.FinalPrice * commissionRate)
                    */
                });

            var netRevenue = new RevenueDetailDto();
            var revenueDataNet = await groupedByDateNet.OrderBy(r => r.Date).ToListAsync();

            var tourNetRevenue = revenueData.Sum(r => r.Tour);
            var tourGuideNetRevenue = revenueData.Sum(r => r.BookingTourGuide);
            var tourWorkshopNetRevenue = revenueData.Sum(r => r.BookingWorkshop);

            var netRevenueByCategoryDto = new RevenueByCategoryDto
            {
                Tour = tourGrossRevenue,
                BookingTourGuide = tourGuideGrossRevenue,
                BookingWorkshop = tourWorkshopRevenue,
            };

            // var totalNetRevenue = revenueData.Sum(r => r.NetRevenue);
            // var totalCommission = revenueData.Sum(r => r.Commission);

            var allDatesNet = Enumerable.Range(0, (toDate.Date - fromDate.Date).Days + 1)
                .Select(d => fromDate.Date.AddDays(d))
                .ToList();

            var dailyNetRevenueStatDto = allDates
                .GroupJoin(revenueData,
                    date => date,
                    data => data.Date,
                    (date, data) => new DailyRevenueStatDto
                    {
                        Date = date,
                        Total = (data.FirstOrDefault()?.Tour ?? 0m)
                        + (data.FirstOrDefault()?.BookingTourGuide ?? 0m)
                        + (data.FirstOrDefault()?.BookingWorkshop ?? 0m),
                        Tour = data.FirstOrDefault()?.Tour ?? 0m,
                        BookingTourGuide = data.FirstOrDefault()?.BookingTourGuide ?? 0m,
                        BookingWorkshop = data.FirstOrDefault()?.BookingWorkshop ?? 0m
                    })
                .OrderBy(r => r.Date)
                .ToList();

            netRevenue.Total = revenueByCategoryDto.Tour + revenueByCategoryDto.BookingTourGuide + revenueByCategoryDto.BookingWorkshop;
            netRevenue.ByCategory = revenueByCategoryDto;
            netRevenue.DailyStats = completeRevenueData;

            result.NetRevenue = netRevenue;
            return result;
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

    private decimal GetCommissionPercent(BookingType bookingType, DateTime applyDate)
    {
        var commissionType = bookingType switch
        {
            BookingType.TourGuide => CommissionType.TourGuideCommission,
            BookingType.Workshop => CommissionType.CraftVillageCommission,
            _ => throw CustomExceptionFactory.CreateNotFoundError("Commission type mapping")
        };

        var commissionRate = _unitOfWork.CommissionRateRepository
            .ActiveEntities
            .Where(c => c.Type == commissionType
                     && c.EffectiveDate <= applyDate
                     && (!c.ExpiryDate.HasValue || applyDate <= c.ExpiryDate))
            .OrderByDescending(c => c.EffectiveDate)
            .FirstOrDefault();

        if (commissionRate == null)
            throw CustomExceptionFactory.CreateNotFoundError("Commission rate");

        return commissionRate.RateValue / 100m;
    }

    public async Task<AdminRevenueDataDto> GetAdminRevenueStatisticsAsync(DateTime fromDate, DateTime toDate)
    {
        try
        {
            var adjustedFromDate = fromDate.Date;
            var adjustedToDate = toDate.Date.AddDays(1).AddTicks(-1);

            var result = new AdminRevenueDataDto();

            var filteredBookings = _unitOfWork.BookingRepository
                .ActiveEntities
                .Where(b => b.StartDate >= adjustedFromDate && b.StartDate <= adjustedToDate);

            var grossBookings = await filteredBookings
                .Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed)
                .ToListAsync();

            var groupedByDateGross = grossBookings
                .GroupBy(b => b.StartDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Tour = g.Where(b => b.BookingType == BookingType.Tour).Sum(b => b.FinalPrice),
                    BookingTourGuide = g.Where(b => b.BookingType == BookingType.TourGuide)
                            .Sum(b => b.FinalPrice * GetCommissionPercent(BookingType.TourGuide, b.BookingDate.DateTime)),

                    BookingWorkshop = g.Where(b => b.BookingType == BookingType.Workshop)
                            .Sum(b => b.FinalPrice * GetCommissionPercent(BookingType.Workshop, b.BookingDate.DateTime)),
                })
            .ToList();

            // -------------------------------------------------- Gross revenue
            var revenueByCategoryDto = new AdminRevenueByCategoryDto
            {
                Tour = groupedByDateGross.Sum(r => r.Tour),
                CommissionTourGuide = groupedByDateGross.Sum(r => r.BookingTourGuide),
                CommissionWorkshop = groupedByDateGross.Sum(r => r.BookingWorkshop)
            };

            var allDates = Enumerable.Range(0, (toDate.Date - fromDate.Date).Days + 1)
                .Select(d => fromDate.Date.AddDays(d))
                .ToList();

            var completeRevenueData = allDates
                .GroupJoin(groupedByDateGross,
                    date => date,
                    data => data.Date,
                    (date, data) => new AdminDailyStatDto
                    {
                        Date = date,
                        Total =
                                (data.FirstOrDefault()?.Tour ?? 0m)
                            + (data.FirstOrDefault()?.BookingTourGuide ?? 0m)
                            + (data.FirstOrDefault()?.BookingWorkshop ?? 0m),
                        Tour = data.FirstOrDefault()?.Tour ?? 0m,
                        CommissionTourGuide = data.FirstOrDefault()?.BookingTourGuide ?? 0m,
                        CommissionWorkshop = data.FirstOrDefault()?.BookingWorkshop ?? 0m
                    })
                .OrderBy(r => r.Date)
                .ToList();

            result.GrossRevenue = new AdminRevenueDto
            {
                Total = revenueByCategoryDto.Tour + revenueByCategoryDto.CommissionTourGuide + revenueByCategoryDto.CommissionWorkshop,
                ByCategory = revenueByCategoryDto,
                DailyStats = completeRevenueData
            };

            // -------------------------------------------------- Net Revenue 
            var completedBookings = await filteredBookings
                .Where(b => b.Status == BookingStatus.Completed)
                .ToListAsync();

            var groupedByDateNet = completedBookings
                .GroupBy(b => b.StartDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Tour = g.Where(b => b.BookingType == BookingType.Tour).Sum(b => b.FinalPrice),
                    BookingTourGuide = g.Where(b => b.BookingType == BookingType.TourGuide)
                                        .Sum(b => b.FinalPrice * GetCommissionPercent(BookingType.TourGuide, b.BookingDate.DateTime)),
                    BookingWorkshop = g.Where(b => b.BookingType == BookingType.Workshop)
                                        .Sum(b => b.FinalPrice * GetCommissionPercent(BookingType.Workshop, b.BookingDate.DateTime)),
                })
                .ToList();

            var netRevenueByCategoryDto = new AdminRevenueByCategoryDto
            {
                Tour = groupedByDateNet.Sum(r => r.Tour),
                CommissionTourGuide = groupedByDateNet.Sum(r => r.BookingTourGuide),
                CommissionWorkshop = groupedByDateNet.Sum(r => r.BookingWorkshop)
            };

            var dailyNetRevenueStatDto = allDates
                .GroupJoin(groupedByDateNet,
                    date => date,
                    data => data.Date,
                    (date, data) => new AdminDailyStatDto
                    {
                        Date = date,
                        Total = (data.FirstOrDefault()?.Tour ?? 0m)
                        + (data.FirstOrDefault()?.BookingTourGuide ?? 0m)
                        + (data.FirstOrDefault()?.BookingWorkshop ?? 0m),
                        Tour = data.FirstOrDefault()?.Tour ?? 0m,
                        CommissionTourGuide = data.FirstOrDefault()?.BookingTourGuide ?? 0m,
                        CommissionWorkshop = data.FirstOrDefault()?.BookingWorkshop ?? 0m
                    })
                .OrderBy(r => r.Date)
                .ToList();

            result.NetRevenue = new AdminRevenueDto
            {
                Total = netRevenueByCategoryDto.Tour + netRevenueByCategoryDto.CommissionTourGuide + netRevenueByCategoryDto.CommissionWorkshop,
                ByCategory = netRevenueByCategoryDto,
                DailyStats = dailyNetRevenueStatDto
            };

            return result;
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
                    WorkshopCount = g.Count(b => b.WorkshopScheduleId.HasValue && b.BookingType == BookingType.Workshop)
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
                        BookingWorkshop = data.FirstOrDefault()?.WorkshopCount ?? 0
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
                        DepartureDate = b.TourSchedule.DepartureDate,
                        UserId = b.UserId,
                        b.TourId,
                        TourName = s.Tour.Name,
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
                UserId = b.UserId,
                DepartureDate = b.DepartureDate,
                UserName = b.UserName,
                TourId = b.TourId ?? Guid.Empty,
                TourName = b.TourName,
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
                    b.TourScheduleId,
                    DepartureDate = b.TourSchedule.DepartureDate,
                    UserId = b.UserId,
                    b.TourId,
                    TourName = b.Tour.Name,
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
                ScheduleId = b.TourScheduleId ?? Guid.Empty,
                DepartureDate = b.DepartureDate,
                UserId = b.UserId,
                TourId = b.TourId ?? Guid.Empty,
                TourName = b.TourName,
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
                    tg.MaxParticipants,
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

    public async Task<TourStatisticDto> GetTourBookingStatisticAsync(Guid tourId)
    {
        try
        {
            var isAdminOrModerator = _userContextService.HasAnyRoleOrAnonymous(AppRole.ADMIN, AppRole.MODERATOR);

            if (!isAdminOrModerator)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var query = _unitOfWork.TourRepository.ActiveEntities
                .Include(t => t.Bookings)
                    .ThenInclude(b => b.User)
                .Where(t => t.Id == tourId);

            var bookingEntities = await _unitOfWork.BookingRepository.ActiveEntities
                .Where(b => b.TourId == tourId)
                .Select(b => new
                {
                    b.Id,
                    b.UserId,
                    UserName = b.User != null ? b.User.FullName : string.Empty,
                    b.TourId,
                    TourName = b.Tour != null ? b.Tour.Name : string.Empty,
                    b.TourScheduleId,
                    DepartureDate = b.TourSchedule != null ? b.TourSchedule.DepartureDate : DateTime.MinValue,
                    b.Status,
                    b.BookingType,
                    b.BookingDate,
                    b.StartDate,
                    b.EndDate,
                    b.PaymentLinkId,
                    b.CancelledAt,
                    b.PromotionId,
                    b.OriginalPrice,
                    b.DiscountAmount,
                    b.FinalPrice,
                    b.ContactName,
                    b.ContactAddress,
                    b.ContactEmail,
                    b.ContactPhone,
                    Participants = b.Participants.Select(p => new
                    {
                        p.Id,
                        p.BookingId,
                        p.Type,
                        Quantity = 1,
                        p.PricePerParticipant,
                        p.FullName,
                        p.Gender,
                        GenderText = _enumService.GetEnumDisplayName<Gender>(p.Gender),
                        p.DateOfBirth
                    }).ToList()
                })
                .OrderBy(b => b.BookingDate)
                .ToListAsync();

            var bookings = bookingEntities
                .Select(b => new BookingDataModel
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    UserName = b.UserName,
                    TourId = b.TourId,
                    TourName = b.TourName,
                    TourScheduleId = b.TourScheduleId,
                    DepartureDate = b.DepartureDate,
                    Status = b.Status,
                    StatusText = _enumService.GetEnumDisplayName<BookingStatus>(b.Status),
                    BookingType = b.BookingType,
                    BookingTypeText = _enumService.GetEnumDisplayName<BookingType>(b.BookingType),
                    BookingDate = b.BookingDate,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    PaymentLinkId = b.PaymentLinkId,
                    PromotionId = b.PromotionId,
                    CancelledAt = b.CancelledAt,
                    OriginalPrice = b.OriginalPrice,
                    DiscountAmount = b.DiscountAmount,
                    FinalPrice = b.FinalPrice,
                    ContactName = b.ContactName,
                    ContactAddress = b.ContactAddress,
                    ContactEmail = b.ContactEmail,
                    ContactPhone = b.ContactPhone,
                    Participants = b.Participants.Select(p => new BookingParticipantDataModel
                    {
                        Id = p.Id,
                        BookingId = p.BookingId,
                        Type = p.Type,
                        Quantity = 1,
                        PricePerParticipant = p.PricePerParticipant,
                        FullName = p.FullName,
                        Gender = p.Gender,
                        GenderText = _enumService.GetEnumDisplayName<Gender>(p.Gender),
                        DateOfBirth = p.DateOfBirth
                    }).ToList()
                })
                .ToList();

            var tourStatistic = await query
                .Select(t => new TourStatisticDto
                {
                    // TourId = t.Id,
                    // TourName = t.Name,
                    // TourStatus = t.Status.ToString(),
                    TotalBookings = t.Bookings.Count,
                    PendingBookings = t.Bookings.Count(b => b.Status == BookingStatus.Pending),
                    ConfirmedBookings = t.Bookings.Count(b => b.Status == BookingStatus.Confirmed),
                    CancelledBookings = t.Bookings.Count(b => b.Status == BookingStatus.Cancelled),
                    ExpiredBookings = t.Bookings.Count(b => b.Status == BookingStatus.Expired),
                    CancelledByProviderBookings = t.Bookings.Count(b => b.Status == BookingStatus.CancelledByProvider),
                    CompletedBookings = t.Bookings.Count(b => b.Status == BookingStatus.Completed),
                    CompletionRate = t.Bookings.Any() ? (t.Bookings.Count(b => b.Status == BookingStatus.Completed) * 100.0 / t.Bookings.Count) : 0,
                    TotalRevenue = t.Bookings
                        .Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed)
                        .Sum(b => b.FinalPrice),
                    ConfirmedRevenue = t.Bookings
                        .Where(b => b.Status == BookingStatus.Confirmed)
                        .Sum(b => b.FinalPrice),
                    CompletedRevenue = t.Bookings
                        .Where(b => b.Status == BookingStatus.Completed)
                        .Sum(b => b.FinalPrice),
                    LostRevenue = t.Bookings
                        .Where(b => b.Status == BookingStatus.Cancelled ||
                                    b.Status == BookingStatus.Expired ||
                                    b.Status == BookingStatus.CancelledByProvider)
                        .Sum(b => b.FinalPrice),
                    Bookings = bookings,
                })
                .FirstOrDefaultAsync();

            if (tourStatistic == null)
            {
                throw CustomExceptionFactory.CreateNotFoundError("Tour");
            }

            return tourStatistic;
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

    public async Task<TourStatisticDto> GetTourScheduleBookingStatisticAsync(Guid tourScheduleId)
    {
        try
        {
            var isAdminOrModerator = _userContextService.HasAnyRoleOrAnonymous(AppRole.ADMIN, AppRole.MODERATOR);

            if (!isAdminOrModerator)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var query = _unitOfWork.TourRepository.ActiveEntities
                .Include(t => t.Bookings)
                    .ThenInclude(b => b.User)
                .Include(t => t.TourSchedules)
                .Where(t => t.TourSchedules.Any(ts => ts.Id == tourScheduleId));

            var bookingEntities = await _unitOfWork.BookingRepository.ActiveEntities
                .Where(b => b.TourScheduleId == tourScheduleId)
                .Select(b => new
                {
                    b.Id,
                    b.UserId,
                    UserName = b.User != null ? b.User.FullName : string.Empty,
                    b.TourId,
                    TourName = b.Tour != null ? b.Tour.Name : string.Empty,
                    b.TourScheduleId,
                    DepartureDate = b.TourSchedule != null ? b.TourSchedule.DepartureDate : DateTime.MinValue,
                    b.Status,
                    b.BookingType,
                    b.BookingDate,
                    b.StartDate,
                    b.EndDate,
                    b.PaymentLinkId,
                    b.CancelledAt,
                    b.PromotionId,
                    b.OriginalPrice,
                    b.DiscountAmount,
                    b.FinalPrice,
                    b.ContactName,
                    b.ContactAddress,
                    b.ContactEmail,
                    b.ContactPhone,
                    Participants = b.Participants.Select(p => new
                    {
                        p.Id,
                        p.BookingId,
                        p.Type,
                        Quantity = 1,
                        p.PricePerParticipant,
                        p.FullName,
                        p.Gender,
                        GenderText = _enumService.GetEnumDisplayName<Gender>(p.Gender),
                        p.DateOfBirth
                    }).ToList()
                })
                .OrderBy(b => b.BookingDate)
                .ToListAsync();

            var bookings = bookingEntities
                .Select(b => new BookingDataModel
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    UserName = b.UserName,
                    TourId = b.TourId,
                    TourName = b.TourName,
                    TourScheduleId = b.TourScheduleId,
                    DepartureDate = b.DepartureDate,
                    Status = b.Status,
                    StatusText = _enumService.GetEnumDisplayName<BookingStatus>(b.Status),
                    BookingType = b.BookingType,
                    BookingTypeText = _enumService.GetEnumDisplayName<BookingType>(b.BookingType),
                    BookingDate = b.BookingDate,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    PaymentLinkId = b.PaymentLinkId,
                    PromotionId = b.PromotionId,
                    CancelledAt = b.CancelledAt,
                    OriginalPrice = b.OriginalPrice,
                    DiscountAmount = b.DiscountAmount,
                    FinalPrice = b.FinalPrice,
                    ContactName = b.ContactName,
                    ContactAddress = b.ContactAddress,
                    ContactEmail = b.ContactEmail,
                    ContactPhone = b.ContactPhone,
                    Participants = b.Participants.Select(p => new BookingParticipantDataModel
                    {
                        Id = p.Id,
                        BookingId = p.BookingId,
                        Type = p.Type,
                        Quantity = 1,
                        PricePerParticipant = p.PricePerParticipant,
                        FullName = p.FullName,
                        Gender = p.Gender,
                        GenderText = _enumService.GetEnumDisplayName<Gender>(p.Gender),
                        DateOfBirth = p.DateOfBirth
                    }).ToList()
                })
                .ToList();

            var tourStatistic = await query
                .Select(t => new TourStatisticDto
                {
                    // TourId = t.Id,
                    // TourName = t.Name,
                    // TourStatus = t.Status.ToString(),
                    TotalBookings = t.Bookings.Count,
                    PendingBookings = t.Bookings.Count(b => b.Status == BookingStatus.Pending),
                    ConfirmedBookings = t.Bookings.Count(b => b.Status == BookingStatus.Confirmed),
                    CancelledBookings = t.Bookings.Count(b => b.Status == BookingStatus.Cancelled),
                    ExpiredBookings = t.Bookings.Count(b => b.Status == BookingStatus.Expired),
                    CancelledByProviderBookings = t.Bookings.Count(b => b.Status == BookingStatus.CancelledByProvider),
                    CompletedBookings = t.Bookings.Count(b => b.Status == BookingStatus.Completed),
                    CompletionRate = t.Bookings.Any() ? (t.Bookings.Count(b => b.Status == BookingStatus.Completed) * 100.0 / t.Bookings.Count) : 0,
                    TotalRevenue = t.Bookings
                        .Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed)
                        .Sum(b => b.FinalPrice),
                    ConfirmedRevenue = t.Bookings
                        .Where(b => b.Status == BookingStatus.Confirmed)
                        .Sum(b => b.FinalPrice),
                    CompletedRevenue = t.Bookings
                        .Where(b => b.Status == BookingStatus.Completed)
                        .Sum(b => b.FinalPrice),
                    LostRevenue = t.Bookings
                        .Where(b => b.Status == BookingStatus.Cancelled ||
                                    b.Status == BookingStatus.Expired ||
                                    b.Status == BookingStatus.CancelledByProvider)
                        .Sum(b => b.FinalPrice),
                    Bookings = bookings,
                })
                .FirstOrDefaultAsync();

            if (tourStatistic == null)
            {
                throw CustomExceptionFactory.CreateNotFoundError("Tour");
            }

            return tourStatistic;
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

    public async Task<TourStatisticDto> GetWorkshopBookingStatisticAsync(Guid workshopId)
    {
        try
        {
            var isAdminOrModerator = _userContextService.HasAnyRoleOrAnonymous(AppRole.ADMIN, AppRole.MODERATOR);

            if (!isAdminOrModerator)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var query = _unitOfWork.WorkshopRepository.ActiveEntities
                .Include(t => t.Bookings)
                    .ThenInclude(b => b.User)
                .Where(t => t.Id == workshopId);

            var bookingEntities = await _unitOfWork.BookingRepository.ActiveEntities
                .Where(b => b.TourId == workshopId)
                .Select(b => new
                {
                    b.Id,
                    b.UserId,
                    UserName = b.User != null ? b.User.FullName : string.Empty,
                    b.WorkshopId,
                    WorkshopName = b.Workshop != null ? b.Workshop.Name : string.Empty,
                    b.WorkshopScheduleId,
                    b.Status,
                    b.BookingType,
                    b.BookingDate,
                    b.StartDate,
                    b.EndDate,
                    b.PaymentLinkId,
                    b.CancelledAt,
                    b.PromotionId,
                    b.OriginalPrice,
                    b.DiscountAmount,
                    b.FinalPrice,
                    b.ContactName,
                    b.ContactAddress,
                    b.ContactEmail,
                    b.ContactPhone,
                    Participants = b.Participants.Select(p => new
                    {
                        p.Id,
                        p.BookingId,
                        p.Type,
                        Quantity = 1,
                        p.PricePerParticipant,
                        p.FullName,
                        p.Gender,
                        GenderText = _enumService.GetEnumDisplayName<Gender>(p.Gender),
                        p.DateOfBirth
                    }).ToList()
                })
                .OrderBy(b => b.BookingDate)
                .ToListAsync();

            var bookings = bookingEntities
                .Select(b => new BookingDataModel
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    UserName = b.UserName,
                    WorkshopId = b.WorkshopId,
                    WorkshopName = b.WorkshopName,
                    WorkshopScheduleId = b.WorkshopScheduleId,
                    Status = b.Status,
                    StatusText = _enumService.GetEnumDisplayName<BookingStatus>(b.Status),
                    BookingType = b.BookingType,
                    BookingTypeText = _enumService.GetEnumDisplayName<BookingType>(b.BookingType),
                    BookingDate = b.BookingDate,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    PaymentLinkId = b.PaymentLinkId,
                    PromotionId = b.PromotionId,
                    CancelledAt = b.CancelledAt,
                    OriginalPrice = b.OriginalPrice,
                    DiscountAmount = b.DiscountAmount,
                    FinalPrice = b.FinalPrice,
                    ContactName = b.ContactName,
                    ContactAddress = b.ContactAddress,
                    ContactEmail = b.ContactEmail,
                    ContactPhone = b.ContactPhone,
                    Participants = b.Participants.Select(p => new BookingParticipantDataModel
                    {
                        Id = p.Id,
                        BookingId = p.BookingId,
                        Type = p.Type,
                        Quantity = 1,
                        PricePerParticipant = p.PricePerParticipant,
                        FullName = p.FullName,
                        Gender = p.Gender,
                        GenderText = _enumService.GetEnumDisplayName<Gender>(p.Gender),
                        DateOfBirth = p.DateOfBirth
                    }).ToList()
                })
                .ToList();

            var tourStatistic = await query
                .Select(t => new TourStatisticDto
                {
                    // TourId = t.Id,
                    // TourName = t.Name,
                    // TourStatus = t.Status.ToString(),
                    TotalBookings = t.Bookings.Count,
                    PendingBookings = t.Bookings.Count(b => b.Status == BookingStatus.Pending),
                    ConfirmedBookings = t.Bookings.Count(b => b.Status == BookingStatus.Confirmed),
                    CancelledBookings = t.Bookings.Count(b => b.Status == BookingStatus.Cancelled),
                    ExpiredBookings = t.Bookings.Count(b => b.Status == BookingStatus.Expired),
                    CancelledByProviderBookings = t.Bookings.Count(b => b.Status == BookingStatus.CancelledByProvider),
                    CompletedBookings = t.Bookings.Count(b => b.Status == BookingStatus.Completed),
                    CompletionRate = t.Bookings.Any() ? (t.Bookings.Count(b => b.Status == BookingStatus.Completed) * 100.0 / t.Bookings.Count) : 0,
                    TotalRevenue = t.Bookings
                        .Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed)
                        .Sum(b => b.FinalPrice),
                    ConfirmedRevenue = t.Bookings
                        .Where(b => b.Status == BookingStatus.Confirmed)
                        .Sum(b => b.FinalPrice),
                    CompletedRevenue = t.Bookings
                        .Where(b => b.Status == BookingStatus.Completed)
                        .Sum(b => b.FinalPrice),
                    LostRevenue = t.Bookings
                        .Where(b => b.Status == BookingStatus.Cancelled ||
                                    b.Status == BookingStatus.Expired ||
                                    b.Status == BookingStatus.CancelledByProvider)
                        .Sum(b => b.FinalPrice),
                    Bookings = bookings,
                })
                .FirstOrDefaultAsync();

            if (tourStatistic == null)
            {
                throw CustomExceptionFactory.CreateNotFoundError("Tour");
            }

            return tourStatistic;
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

    //public async Task<TourStatisticDto> GetWorkshopScheduleBookingStatisticAsync(Guid workshopScheduleId)
    //{
    //    try
    //    {
    //        var isAdminOrModerator = _userContextService.HasAnyRoleOrAnonymous(AppRole.ADMIN, AppRole.MODERATOR);

    //        if (!isAdminOrModerator)
    //        {
    //            throw CustomExceptionFactory.CreateForbiddenError();
    //        }

    //        var query = _unitOfWork.WorkshopRepository.ActiveEntities
    //            .Include(t => t.Bookings)
    //                .ThenInclude(b => b.User)
    //            .Include(t => t.WorkshopSchedules)
    //            .Where(t => t.WorkshopSchedules.Any(ts => ts.Id == workshopScheduleId));

    //        var bookingEntities = await _unitOfWork.BookingRepository.ActiveEntities
    //            .Where(b => b.WorkshopScheduleId == workshopScheduleId)
    //            .Select(b => new
    //            {
    //                b.Id,
    //                b.UserId,
    //                UserName = b.User != null ? b.User.FullName : string.Empty,
    //                b.WorkshopId,
    //                WorkshopName = b.Workshop != null ? b.Workshop.Name : string.Empty,
    //                b.WorkshopScheduleId,
    //                b.Status,
    //                b.BookingType,
    //                b.BookingDate,
    //                b.StartDate,
    //                b.EndDate,
    //                b.PaymentLinkId,
    //                b.CancelledAt,
    //                b.PromotionId,
    //                b.OriginalPrice,
    //                b.DiscountAmount,
    //                b.FinalPrice,
    //                b.ContactName,
    //                b.ContactAddress,
    //                b.ContactEmail,
    //                b.ContactPhone,
    //                Participants = b.Participants.Select(p => new
    //                {
    //                    p.Id,
    //                    p.BookingId,
    //                    p.Type,
    //                    Quantity = 1,
    //                    p.PricePerParticipant,
    //                    p.FullName,
    //                    p.Gender,
    //                    GenderText = _enumService.GetEnumDisplayName<Gender>(p.Gender),
    //                    p.DateOfBirth
    //                }).ToList()
    //            })
    //            .OrderBy(b => b.BookingDate)
    //            .ToListAsync();

    //        var bookings = bookingEntities
    //            .Select(b => new BookingDataModel
    //            {
    //                Id = b.Id,
    //                UserId = b.UserId,
    //                UserName = b.UserName,
    //                WorkshopId = b.WorkshopId,
    //                WorkshopName = b.WorkshopName,
    //                WorkshopScheduleId = b.WorkshopScheduleId,
    //                Status = b.Status,
    //                StatusText = _enumService.GetEnumDisplayName<BookingStatus>(b.Status),
    //                BookingType = b.BookingType,
    //                BookingTypeText = _enumService.GetEnumDisplayName<BookingType>(b.BookingType),
    //                BookingDate = b.BookingDate,
    //                StartDate = b.StartDate,
    //                EndDate = b.EndDate,
    //                PaymentLinkId = b.PaymentLinkId,
    //                PromotionId = b.PromotionId,
    //                CancelledAt = b.CancelledAt,
    //                OriginalPrice = b.OriginalPrice,
    //                DiscountAmount = b.DiscountAmount,
    //                FinalPrice = b.FinalPrice,
    //                ContactName = b.ContactName,
    //                ContactAddress = b.ContactAddress,
    //                ContactEmail = b.ContactEmail,
    //                ContactPhone = b.ContactPhone,
    //                Participants = b.Participants.Select(p => new BookingParticipantDataModel
    //                {
    //                    Id = p.Id,
    //                    BookingId = p.BookingId,
    //                    Type = p.Type,
    //                    Quantity = 1,
    //                    PricePerParticipant = p.PricePerParticipant,
    //                    FullName = p.FullName,
    //                    Gender = p.Gender,
    //                    GenderText = _enumService.GetEnumDisplayName<Gender>(p.Gender),
    //                    DateOfBirth = p.DateOfBirth
    //                }).ToList()
    //            })
    //            .ToList();

    //        var tourStatistic = await query
    //            .Select(t => new TourStatisticDto
    //            {
    //                TotalBookings = t.Bookings.Count,
    //                PendingBookings = t.Bookings.Count(b => b.Status == BookingStatus.Pending),
    //                ConfirmedBookings = t.Bookings.Count(b => b.Status == BookingStatus.Confirmed),
    //                CancelledBookings = t.Bookings.Count(b => b.Status == BookingStatus.Cancelled),
    //                ExpiredBookings = t.Bookings.Count(b => b.Status == BookingStatus.Expired),
    //                CancelledByProviderBookings = t.Bookings.Count(b => b.Status == BookingStatus.CancelledByProvider),
    //                CompletedBookings = t.Bookings.Count(b => b.Status == BookingStatus.Completed),
    //                CompletionRate = t.Bookings.Any() ? (t.Bookings.Count(b => b.Status == BookingStatus.Completed) * 100.0 / t.Bookings.Count) : 0,
    //                TotalRevenue = t.Bookings
    //                    .Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed)
    //                    .Sum(b => b.FinalPrice),
    //                ConfirmedRevenue = t.Bookings
    //                    .Where(b => b.Status == BookingStatus.Confirmed)
    //                    .Sum(b => b.FinalPrice),
    //                CompletedRevenue = t.Bookings
    //                    .Where(b => b.Status == BookingStatus.Completed)
    //                    .Sum(b => b.FinalPrice),
    //                LostRevenue = t.Bookings
    //                    .Where(b => b.Status == BookingStatus.Cancelled ||
    //                                b.Status == BookingStatus.Expired ||
    //                                b.Status == BookingStatus.CancelledByProvider)
    //                    .Sum(b => b.FinalPrice),
    //                Bookings = bookings,
    //            })
    //            .FirstOrDefaultAsync();

    //        if (tourStatistic == null)
    //        {
    //            throw CustomExceptionFactory.CreateNotFoundError("Tour");
    //        }

    //        return tourStatistic;
    //    }
    //    catch (CustomException)
    //    {
    //        throw;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
    //    }
    //}

    public async Task<TourStatisticDto> GetTourGuideBookingStatisticAsync(Guid tourGuideId)
    {
        try
        {
            var isAdminOrModerator = _userContextService.HasAnyRoleOrAnonymous(AppRole.ADMIN, AppRole.MODERATOR);

            if (!isAdminOrModerator)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var query = _unitOfWork.TourGuideRepository.ActiveEntities
                .Include(t => t.Bookings)
                    .ThenInclude(b => b.User)
                .Where(t => t.Id == tourGuideId);

            var bookingEntities = await _unitOfWork.BookingRepository.ActiveEntities
                .Where(b => b.TourGuideId == tourGuideId)
                .Select(b => new
                {
                    b.Id,
                    b.UserId,
                    UserName = b.User != null ? b.User.FullName : string.Empty,
                    b.TourGuideId,
                    TourGuideName = b.TourGuide.User != null ? b.TourGuide.User.FullName : string.Empty,
                    // b.WorkshopScheduleId,
                    b.Status,
                    b.BookingType,
                    b.BookingDate,
                    b.StartDate,
                    b.EndDate,
                    b.PaymentLinkId,
                    b.CancelledAt,
                    b.PromotionId,
                    b.OriginalPrice,
                    b.DiscountAmount,
                    b.FinalPrice,
                    b.ContactName,
                    b.ContactAddress,
                    b.ContactEmail,
                    b.ContactPhone,
                    Participants = b.Participants.Select(p => new
                    {
                        p.Id,
                        p.BookingId,
                        p.Type,
                        Quantity = 1,
                        p.PricePerParticipant,
                        p.FullName,
                        p.Gender,
                        GenderText = _enumService.GetEnumDisplayName<Gender>(p.Gender),
                        p.DateOfBirth
                    }).ToList()
                })
                .OrderBy(b => b.BookingDate)
                .ToListAsync();

            var bookings = bookingEntities
                .Select(b => new BookingDataModel
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    UserName = b.UserName,
                    WorkshopId = b.TourGuideId,
                    WorkshopName = b.TourGuideName,
                    // WorkshopScheduleId = b.WorkshopScheduleId,
                    Status = b.Status,
                    StatusText = _enumService.GetEnumDisplayName<BookingStatus>(b.Status),
                    BookingType = b.BookingType,
                    BookingTypeText = _enumService.GetEnumDisplayName<BookingType>(b.BookingType),
                    BookingDate = b.BookingDate,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    PaymentLinkId = b.PaymentLinkId,
                    PromotionId = b.PromotionId,
                    CancelledAt = b.CancelledAt,
                    OriginalPrice = b.OriginalPrice,
                    DiscountAmount = b.DiscountAmount,
                    FinalPrice = b.FinalPrice,
                    ContactName = b.ContactName,
                    ContactAddress = b.ContactAddress,
                    ContactEmail = b.ContactEmail,
                    ContactPhone = b.ContactPhone,
                    Participants = b.Participants.Select(p => new BookingParticipantDataModel
                    {
                        Id = p.Id,
                        BookingId = p.BookingId,
                        Type = p.Type,
                        Quantity = 1,
                        PricePerParticipant = p.PricePerParticipant,
                        FullName = p.FullName,
                        Gender = p.Gender,
                        GenderText = _enumService.GetEnumDisplayName<Gender>(p.Gender),
                        DateOfBirth = p.DateOfBirth
                    }).ToList()
                })
                .ToList();

            var tourStatistic = await query
                .Select(t => new TourStatisticDto
                {
                    // TourId = t.Id,
                    // TourName = t.Name,
                    // TourStatus = t.Status.ToString(),
                    TotalBookings = t.Bookings.Count,
                    PendingBookings = t.Bookings.Count(b => b.Status == BookingStatus.Pending),
                    ConfirmedBookings = t.Bookings.Count(b => b.Status == BookingStatus.Confirmed),
                    CancelledBookings = t.Bookings.Count(b => b.Status == BookingStatus.Cancelled),
                    ExpiredBookings = t.Bookings.Count(b => b.Status == BookingStatus.Expired),
                    CancelledByProviderBookings = t.Bookings.Count(b => b.Status == BookingStatus.CancelledByProvider),
                    CompletedBookings = t.Bookings.Count(b => b.Status == BookingStatus.Completed),
                    CompletionRate = t.Bookings.Any() ? (t.Bookings.Count(b => b.Status == BookingStatus.Completed) * 100.0 / t.Bookings.Count) : 0,
                    TotalRevenue = t.Bookings
                        .Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed)
                        .Sum(b => b.FinalPrice),
                    ConfirmedRevenue = t.Bookings
                        .Where(b => b.Status == BookingStatus.Confirmed)
                        .Sum(b => b.FinalPrice),
                    CompletedRevenue = t.Bookings
                        .Where(b => b.Status == BookingStatus.Completed)
                        .Sum(b => b.FinalPrice),
                    LostRevenue = t.Bookings
                        .Where(b => b.Status == BookingStatus.Cancelled ||
                                    b.Status == BookingStatus.Expired ||
                                    b.Status == BookingStatus.CancelledByProvider)
                        .Sum(b => b.FinalPrice),
                    Bookings = bookings,
                })
                .FirstOrDefaultAsync();

            if (tourStatistic == null)
            {
                throw CustomExceptionFactory.CreateNotFoundError("Tour");
            }

            return tourStatistic;
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

    public Task<TourStatisticDto> GetWorkshopScheduleBookingStatisticAsync(Guid workshopScheduleId)
    {
        throw new NotImplementedException();
    }

    // private async Task<decimal> GetCommissionPercentAsync(BookingType bookingType, DateTime bookingDate)
    // {
    //     try
    //     {
    //         var commissionSetting = await _unitOfWork.CommissionSettingsRepository
    //             .ActiveEntities
    //             .Where(c => bookingDate >= c.EffectiveDate)
    //             .OrderByDescending(c => c.EffectiveDate)
    //             .FirstOrDefaultAsync();

    //         if (commissionSetting == null)
    //             throw CustomExceptionFactory.CreateNotFoundError("commission setting");

    //         decimal percent = bookingType switch
    //         {
    //             BookingType.TourGuide => commissionSetting.TourGuideCommissionRate,
    //             BookingType.Workshop => commissionSetting.CraftVillageCommissionRate,
    //             _ => 0m
    //         };

    //         return percent / 100m;
    //     }
    //     catch (CustomException)
    //     {
    //         throw;
    //     }
    //     catch (Exception ex)
    //     {
    //         throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
    //     }
    // }
}
