using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.BookingModels;

namespace Travelogue.Service.BusinessModels.RefundRequestModels;

public class RefundRequestDto : BaseDataModel
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public RefundRequestStatus Status { get; set; }
    public string? Reason { get; set; }
    public string? StatusText { get; set; }
    public string? Note { get; set; }
    public DateTimeOffset RequestedAt { get; set; }
    public DateTimeOffset? RespondedAt { get; set; }
    public decimal RefundAmount { get; set; }

    public BookingDataModel? BookingDataModel { get; set; } = null!;
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