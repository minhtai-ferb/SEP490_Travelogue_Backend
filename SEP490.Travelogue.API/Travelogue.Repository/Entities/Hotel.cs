using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class Hotel : BaseEntity
{
    //public decimal? StarRating { get; set; }
    public decimal? PricePerNight { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }

    public Guid? LocationId { get; set; }

    // Navigation Properties
    public Location? Location { get; set; }
    public ICollection<LocationHotelSuggestion> LocationHotelSuggestions { get; set; } = new List<LocationHotelSuggestion>();
}
