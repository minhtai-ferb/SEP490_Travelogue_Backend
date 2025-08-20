using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.RefundRequestModels;

public class RefundRequestDto : BaseDataModel
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public RefundRequestStatus Status { get; set; }
    public string? StatusText { get; set; }
    public string? Note { get; set; }
    public DateTimeOffset RequestedAt { get; set; }
    public DateTimeOffset? RespondedAt { get; set; }
    public decimal RefundAmount { get; set; }
}

public class RefundRequestDetailDto : BaseDataModel
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public RefundRequestStatus Status { get; set; }
    public string? StatusText { get; set; }
    public string? Note { get; set; }

    public decimal RefundAmount { get; set; }
}