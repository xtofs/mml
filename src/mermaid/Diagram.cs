namespace mermaid;

public class Diagram
{
    public IList<Node> Nodes { get; } = [];
    public IList<Link> Links { get; } = [];

    public Node AddNode(string key, string label, NodeShape shape = NodeShape.Box)
    {
        var node = new Node(key, label, shape);
        Nodes.Add(node);
        return node;
    }

    public Link AddLink(Node source, Node target, string label)
    {
        return AddLink(source.Key, target.Key, label);
    }

    public Link AddLink(string sourceKey, string targetKey, string label)
    {
        var link = new Link(sourceKey, targetKey, label);
        Links.Add(link);
        return link;
    }

    public void WriteTo(string path)
    {
        using var file = File.CreateText(path);
        WriteTo(file);
    }

    public void WriteTo(TextWriter writer)
    {
        writer.WriteLine("```mermaid");
        writer.WriteLine("    graph");
        writer.WriteLine();
        foreach (var node in Nodes)
        {
            node.WriteTo(writer);
        }
        foreach (var link in Links)
        {
            link.WriteTo(writer);
        }
        writer.WriteLine("```");
    }
}