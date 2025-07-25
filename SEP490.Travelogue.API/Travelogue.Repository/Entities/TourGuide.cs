using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class TourGuide : BaseEntity
{
    [Range(1, 5)]
    public int Rating { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    public string? Introduction { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public string LanguageCodes { get; set; } = string.Empty;
    public string TagCodes { get; set; } = string.Empty;

    [NotMapped]
    public List<LanguageEnum> Languages
    {
        get => string.IsNullOrWhiteSpace(LanguageCodes)
            ? new List<LanguageEnum>()
            : LanguageCodes.Split(',').Select(code => (LanguageEnum)int.Parse(code)).ToList();

        set => LanguageCodes = string.Join(",", value.Select(v => (int)v));
    }

    [NotMapped]
    public List<TagEnum> Tags
    {
        get => string.IsNullOrWhiteSpace(TagCodes)
            ? new List<TagEnum>()
            : TagCodes.Split(',').Select(code => (TagEnum)int.Parse(code)).ToList();

        set => TagCodes = string.Join(",", value.Select(v => (int)v));
    }

    public User User { get; set; } = null!;

    public ICollection<TourGuideMapping> TourGuideMappings { get; set; } = new List<TourGuideMapping>();
    public ICollection<PromotionApplicable> PromotionApplicables { get; set; } = new List<PromotionApplicable>();
    public ICollection<TourGuideSchedule>? TourGuideSchedules { get; set; }
    public ICollection<Certification>? Certifications { get; set; }
}
