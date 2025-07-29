using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.TourGuideModels;

public class TourGuideScheduleFilterDto
{
    public ScheduleFilterType FilterType { get; set; } = ScheduleFilterType.All;
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
}
