using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.CraftVillageModels;

public class ReviewCraftVillageRequestDto
{
    [Required]
    public CraftVillageRequestStatus Status { get; set; }
    public string? RejectionReason { get; set; }
}