using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.MediaModel;
using Travelogue.Service.BusinessModels.ReviewModels;
using Travelogue.Service.BusinessModels.TourGuideModels;

namespace Travelogue.Service.BusinessModels.TourModels;

public class TourDetailsResponseDto
{
    public Guid TourId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }
    public int TotalDays { get; set; }
    public TourType? TourType { get; set; }
    public string? TourTypeText { get; set; }
    public string? TotalDaysText { get; set; }
    public decimal AdultPrice { get; set; }
    public decimal ChildrenPrice { get; set; }
    public decimal FinalPrice { get; set; }
    public bool IsDiscount { get; set; }
    public TourStatus Status { get; set; }
    public string? StatusText
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
    public List<TourScheduleResponseDto>? Schedules { get; set; }
    public List<TourGuideDataModel>? TourGuide { get; set; }
    public List<PromotionDto>? Promotions { get; set; } = new List<PromotionDto>();
    public List<TourDayDetail> Days { get; set; } = new List<TourDayDetail>();
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public List<ReviewResponseDto> Reviews { get; set; } = new List<ReviewResponseDto>();
    public List<MediaResponse> Medias { get; set; } = new List<MediaResponse>();
}