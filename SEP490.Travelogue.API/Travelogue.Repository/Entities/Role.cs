using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class Role : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? NormalizedName { get; private set; }
    public string? Description { get; set; }
    public ICollection<UserRole>? UserRoles { get; set; }
    public ICollection<RoleDistrict>? RoleDistricts { get; set; }

    private Role() { } // Dành cho EF Core

    public Role(string name, bool skipFormatting = false)
    {
        SetName(name, skipFormatting);
    }

    public static Role Create(string name, bool skipFormatting = false)
    {
        return new Role(name, skipFormatting);
    }

    public void SetName(string name, bool skipFormatting = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be empty");

        if (skipFormatting)
        {
            Name = name;
            NormalizedName = name.ToUpperInvariant();
        }
        else
        {
            Name = GenerateRoleName(name);
            NormalizedName = GenerateNormalizedRoleName(name);
        }
    }

    private static string GenerateNormalizedRoleName(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "ADMIN";

        string normalized = input.Normalize(NormalizationForm.FormD);
        StringBuilder sb = new StringBuilder();

        foreach (char c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        string noDiacritics = sb.ToString().Normalize(NormalizationForm.FormC);
        string formatted = Regex.Replace(noDiacritics, @"\s+", "_").ToUpperInvariant();

        return $"ADMIN_{formatted}";
    }

    private static string GenerateRoleName(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "Admin";

        input = input.ToLower();

        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
        string titleCase = textInfo.ToTitleCase(input);

        return $"Admin {titleCase}";
    }
}

