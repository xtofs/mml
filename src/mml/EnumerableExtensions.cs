public static class EnumerableExtensions
{
    public static IEnumerable<(T, bool)> WithLast<T>(this IEnumerable<T> enumerable)
    {
        var iter = enumerable.GetEnumerator();
        if (!iter.MoveNext()) { yield break; }
        var previous = iter.Current;
        while (iter.MoveNext())
        {
            yield return (previous, false);
            previous = iter.Current;
        }
        yield return (previous, true);
    }
}