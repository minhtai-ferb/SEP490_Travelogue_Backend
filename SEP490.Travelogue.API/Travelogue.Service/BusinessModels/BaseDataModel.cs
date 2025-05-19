namespace Travelogue.Service.BusinessModels;
public class BaseDataModel
{
    public DateTimeOffset CreatedTime { get; set; }
    public DateTimeOffset LastUpdatedTime { get; set; }
    //public DateTimeOffset? DeletedTime { get; set; }
    public string? CreatedBy { get; set; } = string.Empty;
    public string? CreatedByName { get; set; }
    public string? LastUpdatedBy { get; set; } = string.Empty;
    public string? LastUpdatedByName { get; set; }
    //public string? DeletedBy { get; set; }
    //public bool IsActive { get; set; }
    //public bool IsDeleted { get; set; }
}
