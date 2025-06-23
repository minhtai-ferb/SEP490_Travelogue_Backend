using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.TourGuideModels;

public class TourGuideDataModel
{
    public string Email { get; set; }
    public string UserName { get; set; }
    public Gender Sex { get; set; }
    public string? Address { get; set; }
    public int Rating { get; set; }
    public decimal Price { get; set; }
    public string? Introduction { get; set; }
}
