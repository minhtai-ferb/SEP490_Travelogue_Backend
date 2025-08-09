namespace Travelogue.Service.BusinessModels.BankAccountModels;

public class BankAccountUpdateDto
{
    public string BankName { get; set; }
    public string BankAccountNumber { get; set; }
    public string BankOwnerName { get; set; }
    public bool IsDefault { get; set; }
}
