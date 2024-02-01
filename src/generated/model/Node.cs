namespace model;

public abstract class Node(string name) : INode
{
    public string Name { get; protected set; } = name;

    public abstract string Tag { get; }

    public INode? Parent => this.Links.FirstOrDefault(lnk => lnk.Label == Label.CONTAINED).Target;

    public abstract IEnumerable<(string, object)> Attributes { get; }

    public IList<(string Label, INode Target)> Links { get; } = [];

    public string GetQualifiedName(Model root)
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
            return this.Parent.GetQualifiedName(root) + "." + this.Name;
        }
    }
}
