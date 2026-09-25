using System.Text.Json;

namespace BookyServer.Api.Helpers;

public static class CrossSiteScriptingValidation
{
    private static readonly char[] StartingChars = ['<', '&'];

    public static bool IsDangerousString(string s, out int matchIndex)
    {
        matchIndex = 0;

        var index = 0;
        while (true)
        {
            var match = s.IndexOfAny(StartingChars, index);

            if (match < 0)
            {
                return false;
            }

            if (match == s.Length - 1)
            {
                return false;
            }

            matchIndex = match;

            switch (s[match])
            {
                case '<':
                    if (IsAtoZ(s[match + 1]) || s[match + 1] == '!' || s[match + 1] == '/' || s[match + 1] == '?')
                    {
                        return true;
                    }

                    break;
                case '&':
                    if (s[match + 1] == '#')
                    {
                        return true;
                    }

                    break;
            }

            index = match + 1;
        }
    }

    private static bool IsAtoZ(char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
    }

    public static void AddHeaders(this IHeaderDictionary headers)
    {
        if (headers[Constants.Headers.P3P].IsNullOrEmpty())
        {
            headers.Append(Constants.Headers.P3P, Constants.Headers.P3PValue);
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
