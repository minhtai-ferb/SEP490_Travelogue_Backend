using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.ExperienceModels;

public class ExperienceCreateModel
{
    [Required, StringLength(100)]
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }
    public Guid? LocationId { get; set; }
    public Guid? EventId { get; set; }
    public Guid? DistrictId { get; set; }
    public Guid? TypeExperienceId { get; set; }
}
