using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class WorkshopRecurringRuleRequest : BaseEntity
{
    public List<DayOfWeek> DaysOfWeek { get; set; } = new();
    public ICollection<WorkshopSessionRequest> Sessions { get; set; } = new List<WorkshopSessionRequest>();
}
