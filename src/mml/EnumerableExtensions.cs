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

    public delegate bool Filter<T, R>(T item, [MaybeNullWhen(false)] out R res);

    public static IEnumerable<R> FilterSelect<T, R>(this IEnumerable<T> enumerable, Filter<T, R> filter)
    {
        foreach (var item in enumerable)
        {
            if (filter(item, out var res))
            {
                yield return res;
            }
        }
    }
}