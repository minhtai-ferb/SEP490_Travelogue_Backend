namespace Travelogue.Repository.Entities;

using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

public sealed class WorkshopActivityRequest : BaseEntity
{
    public string Activity { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [Range(1, int.MaxValue)]
    public int DurationMinutes { get; set; }

    [Range(1, int.MaxValue)]
    public int ActivityOrder { get; set; }
}