using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.TourGuideModels;

public class TourGuideDataModel
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public int MaxParticipants { get; set; } = 5;
    public Gender Sex { get; set; }
    public string SexText
    {
        get
        {
            return Sex switch
            {
                Gender.Female => "Nữ",
                Gender.Male => "Nam",
                Gender.Other => "Khác",
                _ => "Unknown"
            };
        }
    }
    public string? Address { get; set; }
    // public double AverageRating { get; set; }
    public decimal Price { get; set; }
    public string? Introduction { get; set; }
    public string? AvatarUrl { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
}
