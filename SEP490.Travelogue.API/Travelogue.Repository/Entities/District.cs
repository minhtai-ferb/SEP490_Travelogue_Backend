using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntitys;

namespace Travelogue.Repository.Entities;

public sealed class District : BaseEntity
{
    [Required, StringLength(100)]
    public required string Name { get; set; }
    public string? FileKey { get; set; }
    public string? Description { get; set; }
    public float? Area { get; set; }
    public ICollection<Event>? Events { get; set; }
    public ICollection<Experience>? Experiences { get; set; }
    public ICollection<Location>? Locations { get; set; }
    public ICollection<RoleDistrict>? RoleDistricts { get; set; }

    //public string? City { get; set; }

    //[Range(-90, 90)]
    //public double Latitude { get; set; }

    //[Range(-180, 180)]
    //public double Longitude { get; set; }

    //[Range(0, 5)]
    //public double Rating { get; set; } = 0;
    //public HeritageRank HeritageRank { get; set; }

    //// Navigation Properties
    //public long? TypeLocationId { get; set; }
    //public TypeLocation? TypeLocation { get; set; }
    //public ICollection<Experience>? Experiences { get; set; }
    //public ICollection<Event>? Activities { get; set; }
}
