using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.WithdrawalRequestModels;

public class WithdrawalRequestFilterDto
{
    public Guid? UserId { get; set; }
    public WithdrawalRequestStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}