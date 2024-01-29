namespace Graph;

public record Label(string Name, bool Direction);

public record struct Link(Label Forward, Label Backward)
{
    public Link(string Forward, string Backward) : this(new Label(Forward, true), new Label(Backward, false)) { }

    public static implicit operator Link((string Forward, string Backward) label) => new(label.Forward, label.Backward);

    public static implicit operator (string Forward, string Backward)(Link label) => new(label.Forward.Name, label.Backward.Name);

    public static readonly Link Containment = ("child", "parent");
}


public interface INode
{
    string Name { get; }
    IList<(INode Target, Label Label)> Links { get; }
    void LinkTo(INode that, Link label);
}

public abstract record Node(string Name) : INode
{
    public virtual IList<(INode Target, Label Label)> Links { get; } = new List<(INode Target, Label Label)>();

    public void LinkTo(INode that, Link label)
    {
        this.Links.Add((that, label.Forward));
        that.Links.Add((this, label.Backward));
    }

    public virtual bool TryFind<T>(string name, [MaybeNullWhen(false)] out T node, Label label = null!) where T : Node
    {
        label ??= Graph.Link.Containment.Forward;
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

// public abstract record IndexedNode(string Name) : Node(Name)
// {
//     private readonly IndexedLinkCollection links = new IndexedLinkCollection();

//     public override IList<(Node Target, Label Label)> Links => links;

//     class IndexedLinkCollection : KeyedCollection<string, (Node Target, Label Label)>
//     {
//         protected override string GetKeyForItem((Node Target, Label Label) lnk) => $"{lnk.Label}\u2014{lnk.Target.Name}";
//     }

//     public override bool TryFind<T>(string name, [MaybeNullWhen(false)] out T node, Label label = null!)
//     {
//         label ??= Graph.Link.Containment.Forward;
//         if (links.TryGetValue($"{label}\u2014{name}", out var n) && n.Target is T t)
//         {
//             node = t; return true;
//         }
//         node = default; return false;
//     }

//     protected override bool PrintMembers(StringBuilder builder) =>
//         base.PrintMembers(builder);
// }
