


record struct Label(string Forward, string Backward)
{
    public static implicit operator Label((string Forward, string Backward) label) => new(label.Forward, label.Backward);
    public static implicit operator (string Forward, string Backward)(Label label) => new(label.Forward, label.Backward);

    public static Label Containment = ("contains", "contained");
}

abstract record Node(string Name)
{
    public virtual IList<(Node Target, string Label)> Links { get; } = [];

    public void Link(Node that, Label label)
    {
        this.Links.Add((that, label.Forward));
        that.Links.Add((this, label.Backward));
    }

    public virtual bool TryFind<T>(string name, [MaybeNullWhen(false)] out T node, string label = null!) where T : Node
    {
        label ??= Label.Containment.Forward;
        var lnk = Links.FirstOrDefault(lnk => lnk.Target.Name == name);
        if (lnk.Target != null && lnk.Target is T target)
        {
            node = target; return true;
        }
        node = default; return false;
    }

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.AppendFormat("Name = {0}", Name);
        if (Links.Count != 0)
        {
            builder.Append(", ");
            builder.AppendJoin(", ", Links.Select(lnk => string.Format("{0}: {1} {2}", lnk.Label, lnk.Target.GetType().Name, lnk.Target.Name)));
        }
        return true;
    }
}

abstract record IndexedNode(string Name) : Node(Name)
{
    private readonly IndexedLinkCollection links = new IndexedLinkCollection();
    public override IList<(Node Target, string Label)> Links => links;

    class IndexedLinkCollection : KeyedCollection<string, (Node Target, string Label)>
    {
        protected override string GetKeyForItem((Node Target, string Label) lnk) => lnk.Target.Name;
    }

    public override bool TryFind<T>(string name, [MaybeNullWhen(false)] out T node, string label = null!)
    {
        label ??= Label.Containment.Forward;
        if (links.TryGetValue(name, out var n) && n.Target is T t)
        {
            node = t; return true;
        }
        node = default; return false;
    }

    protected override bool PrintMembers(StringBuilder builder) =>
        base.PrintMembers(builder);
}

