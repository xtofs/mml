namespace model;

public static class IEnumerableExtensions
{
    public static IEnumerable<(T Item, int Index)> Enumerate<T>(this IEnumerable<T> items)
    {
        var ix = 0;
        foreach (var item in items)
        {
            yield return (item, ix);
            ix += 1;
        }
    }
}
