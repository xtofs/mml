

using System.Collections;

class AdjacentNodesView<T>(Node source, Label label) : IEnumerable<T>
   where T : Node
{
    private readonly Node source = source;
    private readonly Label label = label;

    public T Add(T other)
    {
        source.Link(other, label);
        return other;
    }

    private IEnumerable<T> Filtered => source
        .Links
        .Where(lnk => lnk.Label == label.Forward)
        .Select(lnk => lnk.Target)
        .OfType<T>();

    public IEnumerator<T> GetEnumerator() => Filtered.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
