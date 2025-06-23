using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public class Review : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid TourId { get; set; }

    public string Content { get; set; } = null!;
    public int Rating { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Tour Tour { get; set; } = null!;

    public ICollection<Report> Reports { get; set; } = new List<Report>();
}
