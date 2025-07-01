namespace Travelogue.Service.BusinessModels.TourModels;

public class TourDetailResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int TotalDays { get; set; }
    public List<TourDayDetail> Days { get; set; } = new List<TourDayDetail>();
}

public class TourDayDetail
{
    public int DayNumber { get; set; }
    public List<TourActivity> Activities { get; set; } = new List<TourActivity>();
}

public class TourActivity
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty; // "Location", "Cuisine", "CraftVillage"
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public int DayOrder { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public string? StartTimeFormatted { get; set; }
    public string? EndTimeFormatted { get; set; }
    public string? Duration { get; set; }
    public string? Notes { get; set; }
    public string? ImageUrl { get; set; }
    // public decimal? Rating { get; set; }
}
