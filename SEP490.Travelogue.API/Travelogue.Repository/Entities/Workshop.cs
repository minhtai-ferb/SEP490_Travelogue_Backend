using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class Workshop : BaseEntity
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(500)]
    public string? Content { get; set; }

    public WorkshopStatus Status { get; set; } = WorkshopStatus.Draft;

    [Required]
    public Guid CraftVillageId { get; set; }

    public CraftVillage CraftVillage { get; set; } = null!;
    public ICollection<WorkshopActivity> WorkshopActivities { get; set; } = new List<WorkshopActivity>();
    public ICollection<WorkshopSchedule> WorkshopSchedules { get; set; } = new List<WorkshopSchedule>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    // public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<PromotionApplicable> PromotionApplicables { get; set; } = new List<PromotionApplicable>();
}
