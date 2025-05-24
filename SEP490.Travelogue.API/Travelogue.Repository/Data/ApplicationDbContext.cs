using System.Text;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Data;
//public class ApplicationDbContext : IdentityDbContext<User, IdentityRole, string>
public class ApplicationDbContext : DbContext
{
    // User and Role Management
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RoleDistrict> RoleDistricts { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    public DbSet<UserInterest> UserInterests { get; set; }
    public DbSet<UserAnnouncement> UserAnnouncements { get; set; }

    // Location Management
    public DbSet<Location> Locations { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<DistrictMedia> DistrictMedias { get; set; }
    public DbSet<LocationMedia> LocationMedias { get; set; }
    public DbSet<LocationInterest> LocationInterests { get; set; }
    public DbSet<FavoriteLocation> FavoriteLocations { get; set; }
    public DbSet<LocationCraftVillageSuggestion> LocationCraftVillageSuggestions { get; set; }
    public DbSet<LocationCuisineSuggestion> LocationCuisineSuggestions { get; set; }
    public DbSet<LocationHotelSuggestion> LocationHotelSuggestions { get; set; }

    // Craft Village Management
    public DbSet<CraftVillage> CraftVillages { get; set; }
    public DbSet<CraftVillageMedia> CraftVillageMedia { get; set; }
    public DbSet<CraftVillageInterest> CraftVillageInterests { get; set; }
    public DbSet<TourPlanCraftVillage> TourPlanCraftVillages { get; set; }
    public DbSet<TripPlanCraftVillage> TripPlanCraftVillages { get; set; }

    // Cuisine Management
    public DbSet<Cuisine> Cuisines { get; set; }
    public DbSet<CuisineMedia> CuisineMedias { get; set; }
    public DbSet<CuisineInterest> CuisineInterests { get; set; }
    public DbSet<TourPlanCuisine> TourPlanCuisines { get; set; }
    public DbSet<TripPlanCuisine> TripPlanCuisines { get; set; }

    // Hotel Management
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<HotelMedia> HotelMedias { get; set; }

    // Tour Management
    public DbSet<Tour> Tours { get; set; }
    public DbSet<TourType> TourTypes { get; set; }
    public DbSet<TourInterest> TourInterests { get; set; }
    public DbSet<TourPlan> TourPlans { get; set; }
    public DbSet<TourPlanLocation> TourPlanLocations { get; set; }
    public DbSet<TourGuide> TourGuides { get; set; }
    public DbSet<TourGuideAvailability> TourGuideAvailabilities { get; set; }
    public DbSet<TourGroup> TourGroups { get; set; }
    public DbSet<TourGroupMember> TourGroupMembers { get; set; }
    public DbSet<TourJoinRequest> TourJoinRequests { get; set; }

    // Trip Plan Management
    public DbSet<TripPlan> TripPlans { get; set; }
    public DbSet<TripPlanLocation> TripPlanLocations { get; set; }

    // Event Management
    public DbSet<Event> Events { get; set; }
    public DbSet<TypeEvent> TypeEvents { get; set; }
    public DbSet<EventMedia> EventMedias { get; set; }

    // Experience Management
    public DbSet<Experience> Experiences { get; set; }
    public DbSet<TypeExperience> TypeExperiences { get; set; }
    public DbSet<ExperienceMedia> ExperienceMedias { get; set; }

    // News Management
    public DbSet<News> News { get; set; }
    public DbSet<NewsCategory> NewsCategories { get; set; }
    public DbSet<NewsMedia> NewsMedias { get; set; }

    // Announcement Management
    public DbSet<Announcement> Announcements { get; set; }

    // Financial and Transaction Management
    public DbSet<Order> Orders { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Voucher> Vouchers { get; set; }
    public DbSet<WithdrawalRequest> WithdrawalRequests { get; set; }
    public DbSet<OrderWithdrawal> OrderWithdrawals { get; set; }
    public DbSet<RefundRequest> RefundRequests { get; set; }

    // Feedback and Reporting
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Report> Reports { get; set; }

    // Messaging
    public DbSet<Message> Messages { get; set; }

    // System Configuration
    public DbSet<SystemSetting> SystemSettings { get; set; }

    // General
    public DbSet<Interest> Interests { get; set; }

    //// User and Role Management
    //public DbSet<User> Users { get; set; }
    //public DbSet<Role> Roles { get; set; }
    //public DbSet<UserRole> UserRoles { get; set; }
    //public DbSet<RoleDistrict> RoleDistricts { get; set; }
    //public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    //// Location Management
    //public DbSet<Location> Locations { get; set; }
    //public DbSet<District> Districts { get; set; }
    //public DbSet<DistrictMedia> DistrictMedias { get; set; }
    //public DbSet<TypeLocation> TypeLocations { get; set; }
    //public DbSet<LocationMedia> LocationMedias { get; set; }
    //public DbSet<LocationCuisineSuggestion> LocationCuisineSuggestions { get; set; }
    //public DbSet<LocationCraftVillageSuggestion> LocationCraftVillageSuggestions { get; set; }
    //public DbSet<FavoriteLocation> FavoriteLocations { get; set; }

    //// Experience Management
    //public DbSet<Experience> Experiences { get; set; }
    //public DbSet<TypeExperience> TypeExperiences { get; set; }
    //public DbSet<ExperienceMedia> ExperienceMedias { get; set; }

    //// Event Management
    //public DbSet<Event> Events { get; set; }
    //public DbSet<TypeEvent> TypeEvents { get; set; }
    //public DbSet<EventMedia> EventMedias { get; set; }

    //// CraftVillage and Dining
    //public DbSet<Hotel> Hotels { get; set; }
    //public DbSet<HotelMedia> HotelMedias { get; set; }
    //public DbSet<CraftVillage> CraftVillages { get; set; }
    //public DbSet<CraftVillageMedia> CraftVillageMedias { get; set; }
    //public DbSet<Cuisine> Cuisines { get; set; }
    //public DbSet<CuisineMedia> CuisineMedias { get; set; }

    //// News and Media
    //public DbSet<News> News { get; set; }
    //public DbSet<NewsCategory> NewsCategories { get; set; }
    //public DbSet<NewsMedia> NewsMedias { get; set; }

    //// moi them
    //public DbSet<Announcement> Announcements { get; set; }
    //public DbSet<Interest> Interests { get; set; }
    //public DbSet<TourInterest> TourInterests { get; set; }
    //public DbSet<LocationInterest> LocationInterests { get; set; }
    //public DbSet<CraftVillageInterest> CraftVillageInterests { get; set; }
    //public DbSet<CuisineInterest> CuisineInterests { get; set; }
    //public DbSet<TripPlan> TripPlans { get; set; }
    //public DbSet<TripPlanCraftVillage> TripPlanCraftVillages { get; set; }
    //public DbSet<TripPlanCuisine> TripPlanCuisines { get; set; }
    //public DbSet<Order> Orders { get; set; }
    ////public DbSet<OrderType> OrderTypes { get; set; }
    //public DbSet<Transaction> Transactions { get; set; }
    //public DbSet<Report> Reports { get; set; }
    //public DbSet<Review> Reviews { get; set; }
    //public DbSet<TourGroup> TourGroups { get; set; }
    //public DbSet<TourGroupMember> TourGroupMembers { get; set; }
    //public DbSet<Tour> Tours { get; set; }
    //public DbSet<TourPlan> TourPlans { get; set; }
    //public DbSet<TourType> TourTypes { get; set; }
    ////public DbSet<CuisineType> CuisineTypes { get; set; }
    //public DbSet<TourGuide> TourGuides { get; set; }
    //public DbSet<SystemSetting> SystemSettings { get; set; }
    //public DbSet<Message> Messages { get; set; }
    //public DbSet<UserInterest> UserInterests { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Experience>()
            .HasOne(a => a.Location)
            .WithMany(l => l.Experiences)
            .HasForeignKey(a => a.LocationId)
            .OnDelete(DeleteBehavior.Restrict); // Hoặc DeleteBehavior.NoAction

        modelBuilder.Entity<Experience>()
            .HasOne(a => a.Event)
            .WithMany(act => act.Experiences)
            .HasForeignKey(a => a.EventId)
            .OnDelete(DeleteBehavior.Restrict); // Hoặc DeleteBehavior.NoAction

        modelBuilder.Entity<Event>()
            .HasOne(a => a.Location)
            .WithMany(l => l.Activities)
            .HasForeignKey(a => a.LocationId)
            .OnDelete(DeleteBehavior.Restrict); // Tránh vòng lặp xóa

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
