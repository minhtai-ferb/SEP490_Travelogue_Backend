using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;
public sealed class WorkshopException : BaseEntity
{
    [Required]
    public Guid WorkshopId { get; set; }
    public Workshop Workshop { get; set; } = null!;

    public DateTime Date { get; set; }
    public string? Reason { get; set; }
}
