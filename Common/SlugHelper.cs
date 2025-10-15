using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace BackendAPI.Common;

public static class SlugHelper
{
    public static string ToUrlSlug(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "";

        // Gör om till gemener
        text = text.ToLowerInvariant().Trim();

        // Ersätt svenska tecken manuellt
        text = text
            .Replace("å", "a")
            .Replace("ä", "a")
            .Replace("ö", "o");

        // Ta bort accenttecken och diakritiska tecken (även för andra språk)
        var normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var c in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(c);
            if (category != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        text = sb.ToString().Normalize(NormalizationForm.FormC);

        // Byt ut alla icke a-z, 0-9, mellanslag eller bindestreck
        text = Regex.Replace(text, @"[^a-z0-9\s-]", "");

        // Byt ut mellanslag mot bindestreck
        text = Regex.Replace(text, @"\s+", "-");

        // Ta bort dubbla bindestreck
        text = Regex.Replace(text, @"-+", "-");

        // Trimma ev. bindestreck i början/slutet
        text = text.Trim('-');

        return text;
    }
}
