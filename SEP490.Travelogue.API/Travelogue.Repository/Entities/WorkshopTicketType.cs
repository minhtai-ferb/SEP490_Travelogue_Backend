using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class WorkshopTicketType : BaseEntity
{
    [Required]
    public Guid WorkshopId { get; set; }
    public Workshop Workshop { get; set; } = null!;

    public TicketType Type { get; set; } = TicketType.Visit;

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    public bool IsCombo { get; set; } = false;

    [Range(1, int.MaxValue)]
    public int DurationMinutes { get; set; }

    public string? Content { get; set; }

    // Nếu là Experience thì có activities
    public ICollection<WorkshopActivity> WorkshopActivities { get; set; } = new List<WorkshopActivity>();
}