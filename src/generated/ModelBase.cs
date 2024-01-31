namespace model;

using System.Runtime;
using mermaid;

public interface INode
{
    string Name { get; }

    string NodeTag { get; }

    IEnumerable<(string, object)> Attributes { get; }

    IList<(Label Label, INode Target)> Links { get; }

    // string FullyQualifiedName { get; }


    string GetQualifiedName(INode root);
}

public abstract class Node : INode
{
    public Node(string name)
    {
        Name = name;
        // _FullyQualifiedName = new Lazy<string>(GetFullyQualifiedName);
    }

    public string Name { get; protected set; }

    public abstract string NodeTag { get; }

    public INode? Parent => this.Links.FirstOrDefault(lnk => lnk.Label == Label.CONTAINS.Inverse).Target;

    public abstract IEnumerable<(string, object)> Attributes { get; }

    public IList<(Label Label, INode Target)> Links { get; } = [];

    // public override string ToString()
    // {
    //     var sb = new StringBuilder();
    //     this.Print(sb);
    //     return sb.ToString();
    // }

    // private void Print(StringBuilder sb, string indent = "")
    // {
    //     sb.AppendFormat("{0}{1}: {2}", indent, Name, typeName);
    //     sb.AppendLine();
    //     foreach (var node in this.Links.Where(lnk => lnk.Label == Label.CONTAINS && lnk.Label.IsForward).Select(lnk => lnk.Target))
    //     {
    //         ((Node)node).Print(sb, indent + "    ");
    //     }
    // }

    // public string FullyQualifiedName => _FullyQualifiedName.Value;


    // private readonly Lazy<string> _FullyQualifiedName;

    // private string GetFullyQualifiedName()
    // {
    //     return this.Parent == null ? this.Name : this.Parent.FullyQualifiedName + "." + Name;
    // }

    public string GetQualifiedName(INode root)
    {
        ArgumentNullException.ThrowIfNull(root);
        return this.Parent == root || this.Parent == null ?
            this.Name :
            this.Parent.GetQualifiedName(root) + "." + Name;
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

        var triples = from nodeAndIndex in nodes
                      let node = nodeAndIndex.Key
                      let ix = nodeAndIndex.Value
                      from lnk in node.Links
                      where lnk.Label.IsForward
                      select (ix, lnk.Label, nodes.TryGetValue(lnk.Target, out var tgt) ? tgt : throw new KeyNotFoundException($"Key for {lnk.Target} not found"));
        foreach (var (source, label, target) in triples)
        {
            var style = label.Name == Label.CONTAINS.Name ? ContainmentStyle : ReferenceStyle;
            diagram.AddLink($"n{source}", $"n{target}", "" /*label.Name*/, style);
        }
        return diagram;
    }

    static readonly LinkStyle ContainmentStyle = new LinkStyle(Tip.None, Line.Normal, 1, Tip.Arrow);
    static readonly LinkStyle ReferenceStyle = new LinkStyle(Tip.None, Line.Dotted, 1, Tip.Arrow);
}

public class Model : Node
{
    public Model() : base("$MODEL_ROOT")
    {
        this.Nodes = new ContainedCollection<Node>(this, (x) => x.Name);
    }

    public ContainedCollection<Node> Nodes { get; }

    public override IEnumerable<(string, object)> Attributes => [];

    public override string NodeTag => "Model";
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
