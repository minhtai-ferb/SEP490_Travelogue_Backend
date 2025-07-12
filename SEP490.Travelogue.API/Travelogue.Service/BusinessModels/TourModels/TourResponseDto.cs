using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.TourModels;

public class TourResponseDto
{
    public Guid TourId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }
    public int TotalDays { get; set; }
    public Guid TourTypeId { get; set; }
    public string TourTypeText { get; set; }
    public string? TotalDaysText { get; set; }
    public decimal AdultPrice { get; set; }
    public decimal ChildrenPrice { get; set; }
    public decimal FinalPrice { get; set; }
    public TourStatus Status { get; set; }
    public string StatusText
    {
        get
        {
            return Status switch
            {
                TourStatus.Draft => "Draft",
                TourStatus.Confirmed => "Confirmed",
                TourStatus.Cancelled => "Cancelled",
                _ => "Unknown"
            };
        }
    }
}
