
public static class StringExtensions
{
    public static string Escape(this String str)
    {
        return string.Create(str.Length, str, (span, val) =>
        {
            for (int i = 0; i < val.Length; i++)
            {
                span[i] = val[i] < 32 ? (char)(val[i] + '\u2400') : val[i];
            }
        });
    }

    public static string Join<T>(this IEnumerable<T> items, string prefix, string separator)
    {
        // .Any() ? ", " : " ")}{string.Join(", ", classifier.Extends)}
        var sb = new StringBuilder();
        var first = true;
        foreach (var item in items)
        {
            if (first) { sb.Append(prefix); } else { sb.Append(separator); }
            sb.Append(item);
            first = false;
        }
        return sb.ToString();
    }
}