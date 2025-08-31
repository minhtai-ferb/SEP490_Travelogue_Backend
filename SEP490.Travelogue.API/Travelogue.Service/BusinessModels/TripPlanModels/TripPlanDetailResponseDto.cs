using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.TripPlanModels;
public class TripPlanDetailResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? PickupAddress { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? ImageUrl { get; set; }
    public int TotalDays { get; set; }
    public Guid UserId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public TripPlanStatus? Status { get; set; }
    public string? StatusText { get; set; }
    public List<TripDayDetail> Days { get; set; } = new List<TripDayDetail>();
}



public class TripDayDetail
{
    public int DayNumber { get; set; }
    public DateTime Date { get; set; }
    public string DateFormatted { get; set; } = string.Empty;
    public List<TripActivity> Activities { get; set; } = new List<TripActivity>();
}

public class TripActivity
{
    public Guid TripPlanLocationId { get; set; }
    public Guid LocationId { get; set; }
    public double MinPrice { get; set; }
    public double MaxPrice { get; set; }
    public string? Type { get; set; } = string.Empty; // "Location", "Cuisine", "CraftVillage"
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? StartTimeFormatted { get; set; }
    public string? EndTimeFormatted { get; set; }
    public string? Duration { get; set; }
    public string? Notes { get; set; }
    public int? Order { get; set; } // Chỉ có cho Location
    public string? ImageUrl { get; set; }
    // public decimal? Rating { get; set; }
}
