using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class WorkshopActivity : BaseEntity
{
    [Required]
    public Guid WorkshopTicketTypeId { get; set; }
    public WorkshopTicketType WorkshopTicketType { get; set; } = null!;

    [Required, MaxLength(200)]
    public string Activity { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public double StartHour { get; set; }
    public double EndHour { get; set; }

    public int ActivityOrder { get; set; }
}
