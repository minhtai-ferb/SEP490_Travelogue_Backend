using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public class Report : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }

    public Guid ReviewId { get; set; }

    public string? Reason { get; set; }
    public ReportStatus Status { get; set; } = ReportStatus.Pending;

    public User User { get; set; } = null!;
    public Review? Review { get; set; }
}
