namespace Travelogue.Service.BusinessModels.TypeEventModels;
public class TypeEventDataModel : BaseDataModel
{
    public Guid Id { get; set; }
    public string TypeName { get; set; } = string.Empty;
}
