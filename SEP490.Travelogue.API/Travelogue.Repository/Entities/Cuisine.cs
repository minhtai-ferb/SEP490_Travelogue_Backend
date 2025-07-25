using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class Cuisine : BaseEntity
{
    public string? CuisineType { get; set; } // Loại âm thực 
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }

    public Guid? LocationId { get; set; }

    public string? SignatureProduct { get; set; }
    public string? CookingMethod { get; set; }

    // Navigation Properties
    public Location Location { get; set; } = null!;
}
