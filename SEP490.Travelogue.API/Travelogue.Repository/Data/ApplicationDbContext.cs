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

    // Location Management
    public DbSet<Location> Locations { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<DistrictMedia> DistrictMedias { get; set; }
    public DbSet<TypeLocation> TypeLocations { get; set; }
    public DbSet<LocationMedia> LocationMedias { get; set; }
    public DbSet<LocationRestaurantSuggestion> LocationRestaurantSuggestions { get; set; }
    public DbSet<LocationHotelSuggestion> LocationHotelSuggestions { get; set; }
    public DbSet<FavoriteLocation> FavoriteLocations { get; set; }

    // Experience Management
    public DbSet<Experience> Experiences { get; set; }
    public DbSet<TypeExperience> TypeExperiences { get; set; }
    public DbSet<ExperienceMedia> ExperienceMedias { get; set; }

    // Event Management
    public DbSet<Event> Events { get; set; }
    public DbSet<TypeEvent> TypeEvents { get; set; }
    public DbSet<EventMedia> EventMedias { get; set; }

    // Hotel and Dining
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<HotelMedia> HotelMedias { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<RestaurantMedia> RestaurantMedias { get; set; }

    // News and Media
    public DbSet<News> News { get; set; }
    public DbSet<NewsCategory> NewsCategories { get; set; }
    public DbSet<NewsMedia> NewsMedias { get; set; }

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
