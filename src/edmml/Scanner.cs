namespace edmml;

public static class Scanner
{
    private static readonly Regex Regex = new(FromEnum<TokenType>(), OPTIONS);

    private static readonly RegexOptions OPTIONS = RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.ExplicitCapture;

    private static string FromEnum<T>() where T : Enum
    {
        var parts =
            from field in typeof(T).GetFields()
            let pattern = field.GetCustomAttribute<RegexAttribute>()?.Pattern
            where !string.IsNullOrEmpty(pattern)
            select (field.Name, Pattern: pattern);

        var result = @"\G(?:" + string.Join("|", from part in parts select $"(?<{part.Name}>{part.Pattern})") + @")";

        // Console.Error.WriteLine("regex={0}", result.Escape());
        return result;
    }

    public static IEnumerable<Token> Scan(string input, bool includeWhitespace = false)
    {
        var regex = Regex;
        var position = new LineInfo(1, 1);
        var match = regex.Match(input);
        while (match.Success)
        {
            if (match.Groups.Cast<Group>().Skip(1).Count(g => g.Success) > 1)
            {
                var msg = string.Join(", ", match.Groups.Cast<Group>().Skip(1).Where(g => g.Success).Select(g => g.Name));
                throw new Exception("match not unique: " + msg);
            }

            var group = match.Groups.Cast<Group>().Skip(1).Single(g => g.Success);
            var type = Enum.Parse<TokenType>(group.Name);
            if (includeWhitespace || type != TokenType.Whitespace && type != TokenType.LineComment)
            {
                yield return new Token(type, match.Value, position);
            }

            position += match.Value;
            match = match.NextMatch();
        }
    }
}
