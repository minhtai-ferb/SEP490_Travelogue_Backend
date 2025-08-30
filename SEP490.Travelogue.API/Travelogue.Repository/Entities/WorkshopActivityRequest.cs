namespace Travelogue.Repository.Entities;

using Travelogue.Repository.Bases.BaseEntities;

public sealed class WorkshopActivityRequest : BaseEntity
{
    public string Activity { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TimeSpan StartHour { get; set; }
    public TimeSpan EndHour { get; set; }
    public int ActivityOrder { get; set; }
}