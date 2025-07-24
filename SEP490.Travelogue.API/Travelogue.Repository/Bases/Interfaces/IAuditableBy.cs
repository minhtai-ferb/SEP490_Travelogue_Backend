namespace Travelogue.Repository.Bases.Interfaces;
public interface IAuditableBy
{
    string? CreatedBy { get; set; }
    string? LastUpdatedBy { get; set; }
    string? DeletedBy { get; set; }
}

