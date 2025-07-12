using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.TourModels;

public class CreateTourScheduleDto
{
    [Required]
    public DateTime DepartureDate { get; set; }

    [Range(1, int.MaxValue)]
    public int MaxParticipant { get; set; }

    [Range(1, int.MaxValue)]
    public int TotalDays { get; set; }

    [Range(0, double.MaxValue)]
    public decimal AdultPrice { get; set; }

    [Range(0, double.MaxValue)]
    public decimal ChildrenPrice { get; set; }
}
