namespace model;

public interface INode
{
    string Name { get; }

    string Tag { get; }

    IEnumerable<(string, object)> Attributes { get; }

    IList<(string Label, INode Target)> Links { get; }

    string GetQualifiedName(Model root);
}
