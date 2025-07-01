using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;

namespace Travelogue.Repository.Utils;

public static class Utils
{
    public static string RemoveVietnameseTone(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var c in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    public static string GetDisplayName(this Enum value)
    {
        if (value == null)
            return string.Empty;

        // Try to get the DisplayAttribute name if it exists
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttributes(typeof(DisplayAttribute), false)
                              .FirstOrDefault() as DisplayAttribute;
        return attribute?.Name ?? value.ToString();
    }
}
