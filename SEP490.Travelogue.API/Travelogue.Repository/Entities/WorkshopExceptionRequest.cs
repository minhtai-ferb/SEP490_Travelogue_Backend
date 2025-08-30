using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public class WorkshopExceptionRequest : BaseEntity
{
    public DateTime Date { get; set; }
    public string? Reason { get; set; }
    public bool IsActive { get; set; } = true;
}