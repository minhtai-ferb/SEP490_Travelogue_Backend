namespace Travelogue.Service.BusinessModels.TourTypeModels;

public class TourTypeDataModel : BaseDataModel
{
    public Guid Id { get; set; }
    public string TypeName { get; set; } = string.Empty;
}
