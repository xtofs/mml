using mermaid;

namespace model;

public interface INode
{
    string Name { get; }

    IList<(Label Label, INode Target)> Links { get; }
}

public class Node : INode
{
    public Node(string name)
    {
        Name = name;
        typeName = this.GetType().Name;
    }

    public string Name { get; }

    private readonly string typeName;

    public IList<(Label Label, INode Target)> Links { get; } = [];

    public override string ToString()
    {
        var sb = new StringBuilder();
        this.Print(sb);
        return sb.ToString();
    }


    private void Print(StringBuilder sb, string indent = "")
    {
        sb.AppendFormat("{0}{1}: {2}", indent, Name, typeName);
        sb.AppendLine();
        foreach (var node in this.Links.Where(lnk => lnk.Label == Label.CONTAINS && lnk.Label.IsForward).Select(lnk => lnk.Target))
        {
            ((Node)node).Print(sb, indent + "    ");
        }
    }


}

public record class ContainedCollection<T>(INode Host, Func<T, string> Selector) : IEnumerable<T> where T : INode
{
    public IEnumerable<T> Items = Host
            .Links
            .Where(lnk => lnk.Label == Label.CONTAINS && lnk.Label.IsForward)
            .Select(lnk => lnk.Target)
            .OfType<T>();

    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

    public S Add<S>(S item) where S : T
    {
        Host.Links.RemoveFirst(lnk => lnk.Label == Label.REFERENCES && lnk.Label.IsForward == true);
        item.Links.RemoveFirst(lnk => lnk.Label == Label.REFERENCES && lnk.Label.IsForward == false);
        Host.Links.Add((Label.CONTAINS, item));
        item.Links.Add((Label.CONTAINS.Inverse, Host));
        return item;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public record class ContainedSingleton<T>(INode Host) where T : INode
{
    private IEnumerable<T> Items => Host
            .Links
            .Where(lnk => lnk.Label == Label.CONTAINS && lnk.Label.IsForward)
            .Select(lnk => lnk.Target)
            .OfType<T>();

    public T? Value => Items.FirstOrDefault();

    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

    public S? Get<S>() where S : T => Host
              .Links
              .Where(lnk => lnk.Label == Label.REFERENCES && lnk.Label.IsForward)
              .Select(lnk => lnk.Target)
              .OfType<S>().SingleOrDefault();

    public S Set<S>(S item) where S : T
    {
        Host.Links.RemoveFirst(lnk => lnk.Label == Label.CONTAINS && lnk.Label.IsForward == true);
        item.Links.RemoveFirst(lnk => lnk.Label == Label.CONTAINS && lnk.Label.IsForward == false);
        Host.Links.Add((Label.CONTAINS, item));
        item.Links.Add((Label.CONTAINS.Inverse, Host));
        return item;
    }
}

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

public class ReferencedSingleton<T>(INode host) where T : INode
{
    private readonly INode Host = host;

    public S? Get<S>() where S : T => Host
               .Links
               .Where(lnk => lnk.Label == Label.REFERENCES && lnk.Label.IsForward)
               .Select(lnk => lnk.Target)
               .OfType<S>()
               .SingleOrDefault();

    public S Set<S>(S item) where S : T
    {
        Host.Links.RemoveFirst(lnk => lnk.Label == Label.REFERENCES && lnk.Label.IsForward == true);
        item.Links.RemoveFirst(lnk => lnk.Label == Label.REFERENCES && lnk.Label.IsForward == false);
        Host.Links.Add((Label.REFERENCES, item));
        item.Links.Add((Label.REFERENCES.Inverse, Host));
        return item;
    }
}

public static class NodeExtensions
{
    /// <summary>
    /// descendants of node (without self)
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<INode> Descendants(this INode node)
    {
        foreach (var direct in node.Links
            .Where(lnk => lnk.Label == Label.CONTAINS && lnk.Label.IsForward)
            .Select(lnk => lnk.Target))
        {
            yield return direct;

            foreach (var descendant in Descendants(direct))
            {
                yield return descendant;
            }
        }
    }

    public static mermaid.Diagram ToDiagram(this Node root)
    {
        var nodes = Descendants(root).Enumerate().ToDictionary();

        var diagram = new mermaid.Diagram();
        foreach (var (node, ix) in nodes)
        {
            var shape = node.GetType() == typeof(Schema) ? NodeShape.RoundedBox : NodeShape.Box;
            diagram.AddNode($"n{ix}", $"{node.GetType().Name}: {node.Name}", shape);
        }
        foreach (var (source, label, target) in
            from nodeIx in nodes
            let n = nodeIx.Key
            let ix = nodeIx.Value
            from lnk in n.Links
            where lnk.Label.IsForward
            select (ix, lnk.Label, nodes[lnk.Target]))
        {
            diagram.AddLink($"n{source}", $"n{target}", label.Name);
        }
        return diagram;
    }
}

public class Model : Node
{
    public Model() : base("$MODEL_ROOT")
    {
        this.Nodes = new ContainedCollection<Node>(this, (x) => x.Name);
    }

    public ContainedCollection<Node> Nodes { get; }
}


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

/// <summary>
/// Label on Links between nodes
/// each Label has a name and an inverse (that itself is a Label)
/// </summary>
public class Label
{
    private Label(bool isForaward, string name) { IsForward = isForaward; Name = name; Inverse = default!; }

    public string Name { get; }

    public bool IsForward { get; }

    public Label Inverse { get; private set; }

    public static Label Create(string forward, string reverse)
    {
        var fwd = new Label(true, forward);
        var rev = new Label(false, reverse);
        fwd.Inverse = rev;
        rev.Inverse = fwd;
        return fwd;
    }

    public static readonly Label CONTAINS = Label.Create("contains", "containted");
    public static readonly Label REFERENCES = Label.Create("references", "referenced");
}