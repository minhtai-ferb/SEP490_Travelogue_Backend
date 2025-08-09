namespace Travelogue.Service.BusinessModels.BankAccountModels;

public class BankAccountDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string BankName { get; set; }
    public string BankAccountNumber { get; set; }
    public string BankOwnerName { get; set; }

    public bool IsDefault { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
