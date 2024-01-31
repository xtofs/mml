namespace model;

internal static class IListExtensions
{
    public static void RemoveFirst<T>(this IList<T> list, Func<T, bool> pred)
    {
        int i = 0;
        while (i < list.Count)
        {
            if (pred(list[i])) { list.RemoveAt(i); break; }
            i += 1;
        }
    }
}
