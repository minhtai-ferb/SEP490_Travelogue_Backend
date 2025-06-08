using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.TourGuideModels;

public class TourGuideDetailResponse
{
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public required string FullName { get; set; } = string.Empty;

    public Gender Sex { get; set; }
    public string? Address { get; set; }
    public string? Introduction { get; set; }
}
