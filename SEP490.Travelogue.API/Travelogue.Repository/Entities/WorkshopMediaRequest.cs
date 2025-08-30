using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class WorkshopMediaRequest : BaseEntity
{
    public string MediaUrl { get; set; } = string.Empty;
    public bool IsThumbnail { get; set; }
}