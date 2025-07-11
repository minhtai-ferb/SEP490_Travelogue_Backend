using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class BookingParticipant : BaseEntity
{
    public Guid BookingId { get; set; }
    public ParticipantType Type { get; set; }
    public int Quantity { get; set; }
    public decimal PricePerParticipant { get; set; }
    public Booking Booking { get; set; } = null!;
}