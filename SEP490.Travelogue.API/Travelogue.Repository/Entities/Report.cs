using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntitys;

namespace Travelogue.Repository.Entities;

public class Report : BaseEntity
{

    [Required]
    public Guid UserId { get; set; }

    public Guid? ReviewId { get; set; }  

    public string Reason { get; set; } = null!;
    public DateTime ReportedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Review? Review { get; set; } 
}
