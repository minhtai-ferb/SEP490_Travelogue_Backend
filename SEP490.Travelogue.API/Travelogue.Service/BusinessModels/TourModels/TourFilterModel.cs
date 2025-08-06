using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.TourModels;

public class TourFilterModel
{
    public string? Name { get; set; }
    public int? TotalDaysMin { get; set; }
    public int? TotalDaysMax { get; set; }
    public TourType? TourType { get; set; }
    public decimal? PriceMin { get; set; }
    public decimal? PriceMax { get; set; }
    public DateTimeOffset CreatedTime { get; set; }
    public DateTimeOffset LastUpdatedTime { get; set; }
}
