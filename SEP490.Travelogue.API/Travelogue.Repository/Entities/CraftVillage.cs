using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class CraftVillage : BaseEntity
{
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }

    public Guid LocationId { get; set; }

    public bool WorkshopsAvailable { get; set; } = false;
    public string? SignatureProduct { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Số năm lịch sử phải là số không âm")]
    public int? YearsOfHistory { get; set; }

    public bool IsRecognizedByUNESCO { get; set; } = false;

    // Navigation Properties
    public Location Location { get; set; } = null!;
    public ICollection<User> Managers { get; set; } = new List<User>();
    public ICollection<LocationCraftVillageSuggestion> LocationCraftVillageSuggestions { get; set; } = new List<LocationCraftVillageSuggestion>();
}
