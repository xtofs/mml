namespace model;

public record class ContainedCollection<T>(INode Host, Func<T, string> Selector) : IEnumerable<T> where T : INode
{
    public IEnumerable<T> Items = Host
            .Links
            .Where(lnk => lnk.Label == Label.CONTAINS)
            .Select(lnk => lnk.Target)
            .OfType<T>();

    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

    public S Add<S>(S item) where S : T
    {
        // Host.Links.RemoveFirst(lnk => lnk.Label != Label.CONTAINS);
        // item.Links.RemoveFirst(lnk => lnk.Label != Label.CONTAINED);
        Host.Links.Add((Label.CONTAINS, item));
        item.Links.Add((Label.CONTAINED, Host));
        return item;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
