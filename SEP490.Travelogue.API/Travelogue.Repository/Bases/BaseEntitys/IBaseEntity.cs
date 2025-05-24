namespace Travelogue.Repository.Bases.BaseEntitys;

public interface IBaseEntity
{
    Guid Id { get; set; }
    DateTimeOffset CreatedTime { get; set; }
    DateTimeOffset LastUpdatedTime { get; set; }
    DateTimeOffset? DeletedTime { get; set; }

    string? CreatedBy { get; set; }
    string? LastUpdatedBy { get; set; }
    string? DeletedBy { get; set; }

    bool IsActive { get; set; }
    bool IsDeleted { get; set; }
}
