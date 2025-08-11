namespace Travelogue.Service.BusinessModels.DashboardModels;

public class TopTourResponseDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public List<TopTourItem> TopTours { get; set; } = new();
}
public class TopTourItem
{
    public Guid TourId { get; set; }
    public string? TourName { get; set; }
    public int BookingCount { get; set; }
}
