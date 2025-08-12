namespace Travelogue.Service.BusinessModels.RefundRequestModels;

public class RefundRequestCreateDto
{
    // public Guid UserId { get; set; }
    public Guid BookingId { get; set; }

    public DateTime RequestDate { get; set; }

    public string? Reason { get; set; }
    public decimal RefundAmount { get; set; }
}
