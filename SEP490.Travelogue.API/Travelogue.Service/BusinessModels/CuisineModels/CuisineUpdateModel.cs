using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.CuisineModels;
public class CuisineUpdateModel
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Content { get; set; }
    public Guid LocationId { get; set; }
    public string? Address { get; set; }
    public string? CuisineType { get; set; } // Loại âm thực 
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
}
