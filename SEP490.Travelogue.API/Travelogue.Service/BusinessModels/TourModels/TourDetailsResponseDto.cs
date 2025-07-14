using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.TourGuideModels;

namespace Travelogue.Service.BusinessModels.TourModels;

public class TourDetailsResponseDto
{
    public Guid TourId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }
    public int TotalDays { get; set; }
    public Guid TourTypeId { get; set; }
    public string? TourTypeName { get; set; }
    public string? TotalDaysText { get; set; }
    public decimal AdultPrice { get; set; }
    public decimal ChildrenPrice { get; set; }
    public decimal FinalPrice { get; set; }
    public bool IsDiscount { get; set; }
    public TourStatus Status { get; set; }
    public string StatusText
    {
        get
        {
            return Status switch
            {
                TourStatus.Draft => "Draft",
                TourStatus.Confirmed => "Confirmed",
                TourStatus.Cancelled => "Cancelled",
                _ => "Unknown"
            };
        }
    }
    // public List<TourPlanLocationResponseDto> Locations { get; set; }
    public List<TourScheduleResponseDto>? Schedules { get; set; }
    public TourGuideDataModel? TourGuide { get; set; }
    public List<PromotionDto>? Promotions { get; set; } = new List<PromotionDto>();
    public List<TourDayDetail> Days { get; set; } = new List<TourDayDetail>();
}
