using System.Text;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Data;

public class ApplicationDbContext : DbContext
{
    // User and Role Management
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    public DbSet<UserAnnouncement> UserAnnouncements { get; set; }
    public DbSet<Wallet> Wallets { get; set; }

    // Interest
    public DbSet<LocationInterest> locationInterests { get; set; }
    public DbSet<TourInterest> TourInterests { get; set; }

    // Location Management
    public DbSet<Location> Locations { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<DistrictMedia> DistrictMedias { get; set; }
    public DbSet<LocationMedia> LocationMedias { get; set; }
    public DbSet<FavoriteLocation> FavoriteLocations { get; set; }
    public DbSet<HistoricalLocation> HistoricalLocations { get; set; }

    // Craft Village Management
    public DbSet<CraftVillage> CraftVillages { get; set; }
    public DbSet<Workshop> Workshops { get; set; }
    public DbSet<WorkshopSchedule> WorkshopSchedules { get; set; }
    public DbSet<WorkshopActivity> WorkshopActivities { get; set; }
    public DbSet<WorkshopMedia> WorkshopMedias { get; set; }
    public DbSet<CraftVillageRequest> CraftVillageRequests { get; set; }

    // Cuisine Management
    public DbSet<Cuisine> Cuisines { get; set; }

    // Tour Management
    public DbSet<TourGuide> TourGuides { get; set; }
    public DbSet<Certification> Certifications { get; set; }
    public DbSet<TourGuideRequest> TourGuideRequests { get; set; }
    public DbSet<RejectionRequest> RejectionRequests { get; set; }
    public DbSet<TourGuideRequestCertification> TourGuideRequestCertifications { get; set; }
    public DbSet<Tour> Tours { get; set; }
    public DbSet<TourSchedule> TourSchedules { get; set; }
    public DbSet<TourPlanLocation> TourPlanLocations { get; set; }
    public DbSet<TourGuideSchedule> TourGuideSchedules { get; set; }
    public DbSet<TourMedia> TourMedias { get; set; }
    // public DbSet<TourGroup> TourGroups { get; set; }
    // public DbSet<TourGroupMember> TourGroupMembers { get; set; }
    // public DbSet<TourJoinRequest> TourJoinRequests { get; set; }

    // Trip Plan Management
    public DbSet<TripPlan> TripPlans { get; set; }
    public DbSet<TripPlanLocation> TripPlanLocations { get; set; }

    // News Management
    public DbSet<News> News { get; set; }
    public DbSet<NewsMedia> NewsMedias { get; set; }

    // Announcement Management
    public DbSet<Announcement> Announcements { get; set; }

    // Financial and Transaction Management
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<TransactionEntry> Transactions { get; set; }
    public DbSet<Promotion> Promotions { get; set; }
    public DbSet<WithdrawalRequest> WithdrawalRequests { get; set; }
    public DbSet<BookingWithdrawal> BookingWithdrawals { get; set; }
    public DbSet<RefundRequest> RefundRequests { get; set; }
    public DbSet<BankAccount> BankAccounts { get; set; }

    // Feedback and Reporting
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Report> Reports { get; set; }

    // Messaging
    public DbSet<Message> Messages { get; set; }

    // System Configuration
    public DbSet<SystemSetting> SystemSettings { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SystemSetting>().HasData(
            new SystemSetting { Id = Guid.NewGuid(), Key = SystemSettingKey.BookingCommissionPercent, Value = "10" }
        );

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict); // để tránh vòng lặp xóa

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Receiver)
            .WithMany(u => u.ReceivedMessages)
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Promotion>()
            .HasIndex(p => p.PromotionCode)
            .IsUnique();

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Lấy tên bảng gốc
            string? tableName = entity.GetTableName();

            if (tableName != null)
            {
                // Đổi tên sang snake_case
                entity.SetTableName(ToSnakeCase(tableName));

                // Duyệt qua tất cả thuộc tính
                foreach (var property in entity.GetProperties())
                {
                    string propertyName = property.Name;
                    property.SetColumnName(ToSnakeCase(propertyName));
                }
            }
        }
    }

    private string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        var stringBuilder = new StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]) && i > 0)
            {
                stringBuilder.Append("_");
            }
            stringBuilder.Append(char.ToLower(input[i]));
        }
        return stringBuilder.ToString();
    }
}
