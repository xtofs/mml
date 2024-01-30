namespace model;

public interface INode
{
    string Name { get; }

    IList<(string Label, bool Direction, INode Target)> Links { get; }
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

    public IList<(string Label, bool Direction, INode Target)> Links { get; } = [];

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
        foreach (var node in this.Links.Where(lnk => lnk.Label == CONTAINS).Select(lnk => lnk.Target))
        {
            ((Node)node).Print(sb, indent + "    ");
        }
    }

    internal const string CONTAINS = "contains";
    internal const string REFERENCES = "references";
}

public record class ContainedCollection<T>(INode Host, Func<T, string> Selector) : IEnumerable<T> where T : INode
{
    public IEnumerable<T> Items = Host
            .Links
            .Where(lnk => lnk.Label == Node.CONTAINS)
            .Select(lnk => lnk.Target)
            .OfType<T>();

    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

    public S Add<S>(S item) where S : T
    {
        Host.Links.RemoveFirst(lnk => lnk.Label == Node.REFERENCES && lnk.Direction == true);
        item.Links.RemoveFirst(lnk => lnk.Label == Node.REFERENCES && lnk.Direction == false);
        Host.Links.Add((Node.CONTAINS, true, item));
        item.Links.Add((Node.CONTAINS, false, Host));
        return item;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public record class ContainedSingleton<T>(INode Host) where T : INode
{
    private IEnumerable<T> Items => Host
            .Links
            .Where(lnk => lnk.Label == Node.CONTAINS)
            .Select(lnk => lnk.Target)
            .OfType<T>();

    public T? Value => Items.FirstOrDefault();

    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

    public S? Get<S>() where S : T => Host
              .Links
              .Where(lnk => lnk.Label == Node.REFERENCES)
              .Select(lnk => lnk.Target)
              .OfType<S>().SingleOrDefault();

    public S Set<S>(S item) where S : T
    {
        Host.Links.RemoveFirst(lnk => lnk.Label == Node.CONTAINS && lnk.Direction == true);
        item.Links.RemoveFirst(lnk => lnk.Label == Node.CONTAINS && lnk.Direction == false);
        Host.Links.Add((Node.CONTAINS, true, item));
        item.Links.Add((Node.CONTAINS, false, Host));
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
               .Where(lnk => lnk.Label == Node.REFERENCES)
               .Select(lnk => lnk.Target)
               .OfType<S>()
               .SingleOrDefault();

    public S Set<S>(S item) where S : T
    {
        Host.Links.RemoveFirst(lnk => lnk.Label == Node.REFERENCES && lnk.Direction == true);
        item.Links.RemoveFirst(lnk => lnk.Label == Node.REFERENCES && lnk.Direction == false);
        Host.Links.Add((Node.REFERENCES, true, item));
        item.Links.Add((Node.REFERENCES, false, Host));
        return item;
    }
}

public static class NodeExtensions
{
    public static IEnumerable<INode> Descendants(this INode node)
    {
        foreach (var direct in node.Links
            .Where(lnk => lnk.Label == Node.CONTAINS)
            .Select(lnk => lnk.Target))
        {
            yield return direct!;
            foreach (var descendant in Descendants(direct))
            {
                yield return descendant;
            }
        }
    }

    public static mermaid.Diagram ToDiagram(this Node root)
    {
        var nodes = Descendants(root).ToArray();

        var diagram = new mermaid.Diagram();
        foreach (var node in nodes)
        {
            diagram.AddNode(node.Name, $"{node.GetType().Name}: {node.Name}");
        }
        foreach (var (s, l, t) in
            from n in nodes
            from lnk in n.Links
            select (n.Name, lnk.Label, lnk.Target.Name))
        {
            diagram.AddLink(s, t, l);
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

