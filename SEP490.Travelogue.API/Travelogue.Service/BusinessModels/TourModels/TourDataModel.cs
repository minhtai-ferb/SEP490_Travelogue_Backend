namespace Travelogue.Service.BusinessModels.TourModels;

public class TourDataModel : BaseDataModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? Content { get; set; }

    public int TotalDays { get; set; }

    public Guid TourTypeId { get; set; }

    public Guid? CurrentVersionId { get; set; }
}
