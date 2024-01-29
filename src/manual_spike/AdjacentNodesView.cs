
namespace Graph;


class AdjacentNodes<T>(INode source, Link label) : IEnumerable<T>
   where T : INode
{
    private readonly INode source = source;
    private readonly Link label = label;

    public S Add<S>(S other) where S : T
    {
        source.LinkTo(other, label);
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
