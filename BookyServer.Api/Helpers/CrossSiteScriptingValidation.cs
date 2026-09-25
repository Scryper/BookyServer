using System.Text.Json;

namespace BookyServer.Api.Helpers;

public static class CrossSiteScriptingValidation
{
	private static readonly char[] StartingChars = { '<', '&' };

    public static bool IsDangerousString(string s, out int matchIndex)
    {
        matchIndex = 0;

        for (var i = 0; ;)
        {

            // Look for the start of one of our patterns 
            var n = s.IndexOfAny(StartingChars, i);

            // If not found, the string is safe
            if (n < 0) return false;

            // If it's the last char, it's safe 
            if (n == s.Length - 1) return false;

            matchIndex = n;

            switch (s[n])
            {
                case '<':
                    // If the < is followed by a letter or '!', it's unsafe (looks like a tag or HTML comment)
                    if (IsAtoZ(s[n + 1]) || s[n + 1] == '!' || s[n + 1] == '/' || s[n + 1] == '?') return true;
                    break;
                case '&':
                    // If the & is followed by a #, it's unsafe (e.g. S) 
                    if (s[n + 1] == '#') return true;
                    break;

            }

            // Continue searching
            i = n + 1;
        }
    }

    private static bool IsAtoZ(char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
    }

    public static void AddHeaders(this IHeaderDictionary headers)
    {
        if (headers["P3P"].IsNullOrEmpty())
        {
            headers.Append("P3P", "CP=\"IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT\"");
        }
    }

    private static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
    {
        return source == null || !source.Any();
    }

    public static string ToJson(this object? value)
    {
        return JsonSerializer.Serialize(value);
    }
}