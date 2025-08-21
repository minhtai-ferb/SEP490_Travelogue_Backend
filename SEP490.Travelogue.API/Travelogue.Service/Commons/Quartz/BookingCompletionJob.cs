using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Commons.Quartz;

public class BookingCompletionJob : IJob
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ITimeService _timeService;

    public BookingCompletionJob(IServiceProvider serviceProvider, ITimeService timeService)
    {
        _serviceProvider = serviceProvider;
        _timeService = timeService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var timeService = scope.ServiceProvider.GetRequiredService<ITimeService>();

                var now = timeService.SystemTimeNow;

                var expiredBookings = await unitOfWork.BookingRepository
                    .ActiveEntities
                    .Include(b => b.TourGuide)
                        .ThenInclude(g => g.User)
                            .ThenInclude(u => u.Wallet)
                    .Include(b => b.Workshop)
                        .ThenInclude(w => w.CraftVillage)
                            .ThenInclude(cv => cv.Owner)
                                .ThenInclude(u => u.Wallet)
                    .Where(b => b.EndDate <= now && b.Status == BookingStatus.Confirmed)
                    .ToListAsync();

                foreach (var booking in expiredBookings)
                {
                    booking.Status = BookingStatus.Completed;
                    unitOfWork.BookingRepository.Update(booking);

                    var commissionRate = await unitOfWork.CommissionRateRepository.ActiveEntities
                        .Where(c =>
                            c.Type == (booking.BookingType == BookingType.TourGuide
                                        ? CommissionType.TourGuideCommission
                                        : CommissionType.CraftVillageCommission)
                            && c.EffectiveDate <= booking.BookingDate.DateTime
                            && (!c.ExpiryDate.HasValue || booking.BookingDate.DateTime <= c.ExpiryDate))
                        .OrderByDescending(c => c.EffectiveDate)
                        .FirstOrDefaultAsync();

                    if (commissionRate == null || booking.FinalPrice <= 0)
                        continue;

                    var commissionPercent = commissionRate.RateValue / 100m;

                    Wallet? targetWallet = null;
                    Guid? targetUserId = null;

                    if (booking.BookingType == BookingType.TourGuide && booking.TourGuide?.User.Wallet != null)
                    {
                        targetWallet = booking.TourGuide.User.Wallet;
                        targetUserId = booking.TourGuide.User.Id;
                    }
                    else if (booking.BookingType == BookingType.Workshop && booking.Workshop?.CraftVillage?.Owner?.Wallet != null)
                    {
                        targetWallet = booking.Workshop.CraftVillage.Owner.Wallet;
                        targetUserId = booking.Workshop.CraftVillage.Owner.Id;
                    }

                    if (targetUserId == null)
                        continue;

                    // chua co vi
                    if (targetWallet == null)
                    {
                        targetWallet = new Wallet
                        {
                            UserId = targetUserId.Value,
                            Balance = 0m,
                            CreatedBy = targetUserId.ToString(),
                            LastUpdatedBy = targetUserId.ToString()
                        };
                        await unitOfWork.WalletRepository.AddAsync(targetWallet);
                    }

                    var commissionAmount = booking.FinalPrice * commissionPercent;

                    if (commissionAmount > 0)
                    {
                        // cộng tiền
                        targetWallet.Balance += commissionAmount;
                        unitOfWork.WalletRepository.Update(targetWallet);

                        // transaction
                        var commissionTransaction = new TransactionEntry
                        {
                            Id = Guid.NewGuid(),
                            WalletId = targetWallet.Id,
                            UserId = targetUserId,
                            PaidAmount = commissionAmount,
                            Type = TransactionType.Booking,
                            TransactionDirection = TransactionDirection.Credit,
                            Status = TransactionStatus.Completed,
                            PaymentStatus = PaymentStatus.Success,
                            Description = $"Hoa hồng cho booking {booking.Id}",
                            Method = "System",
                            TransactionDateTime = now.UtcDateTime,
                            Currency = "VND"
                        };

                        await unitOfWork.TransactionEntryRepository.AddAsync(commissionTransaction);
                    }
                }

                if (expiredBookings.Any())
                {
                    await unitOfWork.SaveAsync();
                    Console.WriteLine("Updated booking hoàn thành + chia hoa hồng");
                }
            }
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
