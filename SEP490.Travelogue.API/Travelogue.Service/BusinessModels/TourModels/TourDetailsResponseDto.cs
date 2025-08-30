using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.MediaModel;
using Travelogue.Service.BusinessModels.ReviewModels;

namespace Travelogue.Service.BusinessModels.TourModels;

public class TourDetailsResponseDto
{
    public Guid TourId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }
    public string? TransportType { get; set; }
    public string? PickupAddress { get; set; }
    public string? StayInfo { get; set; }
    public int TotalDays { get; set; }
    public TourType? TourType { get; set; }
    public string? TourTypeText { get; set; }
    public string? TotalDaysText { get; set; }
    public decimal AdultPrice { get; set; }
    public decimal ChildrenPrice { get; set; }
    public decimal FinalPrice { get; set; }
    public bool IsDiscount { get; set; }
    public TourStatus Status { get; set; }
    public string? StatusText { get; set; }
    public List<TourScheduleResponseDto>? Schedules { get; set; }
    // public List<TourGuideDataModel>? TourGuide { get; set; }
    public List<PromotionDto>? Promotions { get; set; } = new List<PromotionDto>();
    public List<TourDayDetail> Days { get; set; } = new List<TourDayDetail>();
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }

    public TourActivity? StartLocation { get; set; }
    public TourActivity? EndLocation { get; set; }

    public List<ReviewResponseDto> Reviews { get; set; } = new List<ReviewResponseDto>();
    public List<MediaResponse> Medias { get; set; } = new List<MediaResponse>();
}

public class TourDayDetail
{
    public int DayNumber { get; set; }
    public List<TourActivity> Activities { get; set; } = new List<TourActivity>();
}

public class TourActivity
{
    public Guid TourPlanLocationId { get; set; }
    public Guid LocationId { get; set; }
    public string Type { get; set; } = string.Empty; // "Location", "Cuisine", "CraftVillage"
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public ActivityType ActivityType { get; set; }
    public string? ActivityTypeText { get; set; }
    public int DayOrder { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public string? StartTimeFormatted { get; set; }
    public string? EndTimeFormatted { get; set; }
    public string? Duration { get; set; }
    public string? Notes { get; set; }
    public string? ImageUrl { get; set; }
    public float TravelTimeFromPrev { get; set; }
    public float DistanceFromPrev { get; set; }
    public float EstimatedStartTime { get; set; }
    public float EstimatedEndTime { get; set; }
    public TourActivityWorkshopInfo? Workshop { get; set; }
    // public decimal? Rating { get; set; }
}

public class PromotionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal DiscountPercentage { get; set; } = 0;
    public decimal DiscountAmount { get; set; } = 0;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
}

public class TourActivityWorkshopInfo
{
    public Guid? WorkshopId { get; set; }
    public string? WorkshopName { get; set; }

    public Guid? WorkshopTicketTypeId { get; set; }
    public string? WorkshopTicketTypeName { get; set; }
    public int? WorkshopTicketDurationMinutes { get; set; }
    public decimal? WorkshopTicketPrice { get; set; }

    public Guid? WorkshopSessionRuleId { get; set; }
    public TimeSpan? WorkshopSessionStart { get; set; }
    public TimeSpan? WorkshopSessionEnd { get; set; }
    public string? WorkshopSessionTimeFormatted { get; set; }
    public int? WorkshopSessionCapacity { get; set; }
}