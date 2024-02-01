namespace model;

public class ReferencedSingleton<T>(INode host, string label) where T : INode
{
    private readonly INode Host = host;
    private readonly string Label = label;

    public T? Value { get => Get(); }

    public T? Get() => Host
            .Links
            .Where(lnk => lnk.Label == this.Label)
            .Select(lnk => lnk.Target)
            .OfType<T>()
            .SingleOrDefault();

    public S Set<S>(S item) where S : T
    {
        Host.Links.RemoveFirst(lnk => lnk.Label == this.Label);
        Host.Links.Add((this.Label, item));
        return item;
    }
}
