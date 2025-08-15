using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.RefundRequestModels;

public class RefundRequestAdminFilter
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public RefundRequestStatus? Status { get; set; }
    public Guid? UserId { get; set; }
}

public class RefundRequestUserFilter
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public RefundRequestStatus? Status { get; set; }
}