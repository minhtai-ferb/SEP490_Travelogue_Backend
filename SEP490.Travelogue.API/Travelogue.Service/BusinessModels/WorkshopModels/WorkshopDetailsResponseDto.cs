using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.MediaModel;
using Travelogue.Service.BusinessModels.ReviewModels;
using Travelogue.Service.BusinessModels.TourModels;

namespace Travelogue.Service.BusinessModels.WorkshopModels;

public class WorkshopDetailsResponseDto
{
    public Guid WorkshopId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }
    public Guid CraftVillageId { get; set; }
    public string? CraftVillageName { get; set; }
    public WorkshopStatus Status { get; set; }
    public string StatusText { get; set; }
    public List<ActivityResponseDto>? Activities { get; set; }
    public List<PromotionDto>? Promotions { get; set; } = new List<PromotionDto>();
    public List<ScheduleResponseDto>? Schedules { get; set; }
    public List<WorkshopDayDetail> Days { get; set; } = new List<WorkshopDayDetail>();
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public List<ReviewResponseDto> Reviews { get; set; } = new List<ReviewResponseDto>();
    public List<MediaResponse> Medias { get; set; } = new List<MediaResponse>();
}

public class WorkshopDayDetail
{
    public int DayNumber { get; set; }
    public List<WorkshopActivityDtoOLD> Activities { get; set; } = new List<WorkshopActivityDtoOLD>();
}

public class WorkshopActivityDtoOLD
{
    public Guid Id { get; set; }
    public string Activity { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DayOrder { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public string? StartTimeFormatted { get; set; }
    public string? EndTimeFormatted { get; set; }
    public string? Duration { get; set; }
    public string? Notes { get; set; }
    // public string? ImageUrl { get; set; }
    // public decimal? Rating { get; set; }
}