using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class Message : BaseEntity
{
    [Required]
    public Guid ReceiverId { get; set; }

    [Required]
    public Guid SenderId { get; set; }

    [Required, MaxLength(2000)]
    public string Content { get; set; } = string.Empty;

    [DataType(DataType.DateTime)]
    public DateTime SentAt { get; set; }

    [ForeignKey("SenderId")]
    public User? Sender { get; set; }

    [ForeignKey("ReceiverId")]
    public User? Receiver { get; set; }
}
