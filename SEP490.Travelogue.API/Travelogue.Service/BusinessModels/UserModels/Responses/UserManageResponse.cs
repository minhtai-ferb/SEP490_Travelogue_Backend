using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.BankAccountModels;
using Travelogue.Service.BusinessModels.TourGuideModels;
using Travelogue.Service.BusinessModels.TransactionModels;

namespace Travelogue.Service.BusinessModels.UserModels.Responses;

public class UserManageResponse : BaseDataModel
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string UserName { get; set; }
    public bool? EmailConfirmed { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? PhoneNumberConfirmed { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public List<RoleManageDto> Roles { get; set; }
    public Gender Sex { get; set; }
    public string? GenderText { get; set; }
    public string? Address { get; set; }
    public bool? IsEmailVerified { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public List<BankAccountDto> BankAccounts { get; set; } = new();
    public WalletDto Wallet { get; set; } = new();
    public TourGuideInfo? TourGuideInfo { get; set; }
    public CraftVillagesInfo? CraftVillagesInfo { get; set; }
}

public class RoleManageDto
{
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class WalletDto
{
    public decimal UserWalletAmount { get; set; }
    public List<TransactionDto> TransactionDtos { get; set; } = new();
}

public class TourGuideInfo
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    public decimal Price { get; set; }
    public string? Introduction { get; set; }
    public int TotalReviews { get; set; }
    public List<CertificationDto> Certifications { get; set; } = new();
}

public class CraftVillagesInfo
{
    public Guid Id { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? SignatureProduct { get; set; }
    public int? YearsOfHistory { get; set; }
    public bool IsRecognizedByUnesco { get; set; } = false;
    public bool WorkshopsAvailable { get; set; }
    public Guid LocationId { get; set; }
}