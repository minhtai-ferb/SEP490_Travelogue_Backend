using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.DashboardModels;

public class TourGuideBookingResponse
{
    public Guid TourGuideId { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public Gender Sex { get; set; }
    public string SexText { get; set; }
    public string? Address { get; set; }
    // public double AverageRating { get; set; }
    public decimal Price { get; set; }
    public string? Introduction { get; set; }
    public string? AvatarUrl { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public List<TourGuideBookingItem> Bookings { get; set; } = new();
}

public class TourGuideBookingItem
{
    public Guid BookingId { get; set; }
    public string TourName { get; set; }
    public string CustomerName { get; set; }
    public BookingStatus Status { get; set; }
    public string? StatusText { get; set; }

    public BookingType BookingType { get; set; }
    public string? BookingTypeText { get; set; }
    public decimal FinalPrice { get; set; }
}
