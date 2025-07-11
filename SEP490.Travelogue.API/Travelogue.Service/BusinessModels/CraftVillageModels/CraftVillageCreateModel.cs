using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.CraftVillageModels;

public class CraftVillageCreateModel
{
    // [Required, StringLength(100)]
    // public string Name { get; set; } = string.Empty;
    // public string? Description { get; set; }
    // public string? Content { get; set; }
    // public string? Address { get; set; }
    // public decimal? StarRating { get; set; }
    // public decimal? PricePerNight { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public bool WorkshopsAvailable { get; set; } = false;
    public string? SignatureProduct { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Số năm lịch sử phải là số không âm")]
    public int? YearsOfHistory { get; set; }

    public bool IsRecognizedByUNESCO { get; set; } = false;
}
