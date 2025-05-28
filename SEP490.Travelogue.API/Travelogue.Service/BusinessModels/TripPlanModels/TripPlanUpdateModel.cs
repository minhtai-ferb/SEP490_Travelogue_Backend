using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.TripPlanModels;

public class TripPlanUpdateModel
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    public List<TripPlanLocationModel>? Locations { get; set; } = new List<TripPlanLocationModel>();
    public List<TripPlanCuisineModel>? Cuisines { get; set; } = new List<TripPlanCuisineModel>();
    public List<TripPlanCraftVillageModel>? CraftVillages { get; set; } = new List<TripPlanCraftVillageModel>();
}

public class TripPlanLocationModel
{
    public Guid? Id { get; set; }
    public Guid LocationId { get; set; }
    public int Order { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Notes { get; set; }
}

public class TripPlanCuisineModel
{
    public Guid? Id { get; set; }
    public Guid CuisineId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Notes { get; set; }
}

public class TripPlanCraftVillageModel
{
    public Guid? Id { get; set; }
    public Guid CraftVillageId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Notes { get; set; }
}
