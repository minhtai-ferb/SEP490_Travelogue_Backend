using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class WorkshopRecurringRule : BaseEntity
{
    [Required]
    public Guid WorkshopId { get; set; }
    public Workshop Workshop { get; set; } = null!;

    public ICollection<DayOfWeek> DaysOfWeek { get; set; } = new List<DayOfWeek>();
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public ICollection<WorkshopSessionRule> Sessions { get; set; } = new List<WorkshopSessionRule>();
}