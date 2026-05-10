using System.Text.RegularExpressions;

namespace e_Sat_Auction.Common.Extensions;

public static class StringExtensions
{
    public static string ToSemanticCode(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }
     
        string normalized = text
            .Trim()
            .ToLowerInvariant()
            .Replace("ç", "c")
            .Replace("ğ", "g")
            .Replace("ı", "i")
            .Replace("ö", "o")
            .Replace("ş", "s")
            .Replace("ü", "u")
            .Replace(" ", "_");
     
        return Regex.Replace(normalized, @"[^a-z0-9_]", string.Empty);
    }
}