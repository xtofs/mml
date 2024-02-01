using System.Collections.Frozen;

namespace model;

public interface INode
{
    string Name { get; }

    string Tag { get; }

    IEnumerable<(string, object)> Attributes { get; }

    IList<(string Label, INode Target)> Links { get; }

    string GetQualifiedName(INode root);
}

public abstract class Node(string name) : INode
{
    public string Name { get; protected set; } = name;

    public abstract string Tag { get; }

    public INode? Parent => this.Links.FirstOrDefault(lnk => lnk.Label == Label.CONTAINED).Target;

    public abstract IEnumerable<(string, object)> Attributes { get; }

    public IList<(string Label, INode Target)> Links { get; } = [];

    public string GetQualifiedName(INode root)
    {
        ArgumentNullException.ThrowIfNull(root);
        if (this.Parent == null)
        {
            return this.Name;
        }
        else if (this.Parent == root)
        {
            return this.Name;
        }
        else
        {
            var sep = SEPARATORS.TryGetValue(this.Parent.GetType(), out var ch) ? ch : ".";
            return this.Parent.GetQualifiedName(root) + sep + this.Name;
        }
    }

    static readonly FrozenDictionary<System.Type, string> SEPARATORS = new Dictionary<System.Type, string>
    {
        [typeof(Schema)] = ".",
        [typeof(StructuralProperty)] = "::",
        [typeof(EntityType)] = "/",
        [typeof(ComplexType)] = "/",
    }.ToFrozenDictionary();
}

public static class INodeExtensions
{
    public static IEnumerable<INode> Children(this INode node) =>
        node.Links.Where(lnk => lnk.Label == Label.CONTAINS).Select(lnk => lnk.Target);
}