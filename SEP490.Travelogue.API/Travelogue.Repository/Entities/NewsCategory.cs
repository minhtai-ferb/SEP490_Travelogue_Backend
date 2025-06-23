using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class NewsCategory : BaseEntity
{
    public string Category { get; set; } = string.Empty;
    public ICollection<News>? News { get; set; }
}
