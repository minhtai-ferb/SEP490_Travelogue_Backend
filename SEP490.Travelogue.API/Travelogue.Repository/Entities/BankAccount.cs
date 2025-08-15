using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class BankAccount : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(255)]
    public required string BankName { get; set; }

    [Required]
    [MaxLength(50)]
    public required string BankAccountNumber { get; set; }

    [Required]
    [MaxLength(255)]
    public required string BankOwnerName { get; set; }

    public bool IsDefault { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property 
    public User User { get; set; }
}