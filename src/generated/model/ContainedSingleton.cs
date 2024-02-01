namespace model;

public class ContainedSingleton<T> where T : INode, new()
{
    private readonly INode Host;

    public ContainedSingleton(INode host)
    {
        Host = host;
        var item = new T();
        Host.Links.Add((Label.CONTAINS, item));
        item.Links.Add((Label.CONTAINED, Host));
    }

    private IEnumerable<T> Items => Host
            .Links
            .Where(lnk => lnk.Label == Label.CONTAINS)
            .Select(lnk => lnk.Target)
            .OfType<T>();

    // public S? Get<S>() where S : T => Items
    public T? Get() => Items
              .OfType<T>()
              .SingleOrDefault();

    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

    public S Set<S>(S item) where S : T
    {
        // TODO: might break if a node has two ContainedSingletons
        Host.Links.RemoveFirst(lnk => lnk.Label == Label.CONTAINS);
        item.Links.RemoveFirst(lnk => lnk.Label == Label.CONTAINS);
        Host.Links.Add((Label.CONTAINS, item));
        item.Links.Add((Label.CONTAINED, Host));
        return item;
    }
}
