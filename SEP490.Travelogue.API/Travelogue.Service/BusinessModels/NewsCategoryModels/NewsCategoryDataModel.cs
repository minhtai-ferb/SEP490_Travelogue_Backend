namespace Travelogue.Service.BusinessModels.NewsCategoryModels;

public class NewsCategoryDataModel : BaseDataModel
{
    public Guid Id { get; set; }
    public required string Category { get; set; }
}
