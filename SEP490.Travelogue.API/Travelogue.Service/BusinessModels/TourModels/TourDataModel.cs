namespace Travelogue.Service.BusinessModels.TourModels;

public class TourDataModel : BaseDataModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? Content { get; set; }

    public int TotalDays { get; set; }
    public decimal AdultPrice { get; set; }
    public decimal ChildrenPrice { get; set; }
    public decimal PriceFinal { get; set; }
    public bool IsDiscount { get; set; }

    public Guid TourTypeId { get; set; }
    public string? TourTypeText { get; set; }

    public Guid? CurrentVersionId { get; set; }
}
