using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.TourGuideModels;

public class TourGuideFilterRequest
{
    public string? FullName { get; set; }

    public int? MinRating { get; set; }
    public int? MaxRating { get; set; }

    public Gender? Gender { get; set; }

    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}