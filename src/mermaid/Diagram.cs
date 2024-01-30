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

    internal const string INDENT = "    ";

    public void WriteTo(TextWriter writer)
    {
        // https://mermaid.js.org/syntax/flowchart.html
        writer.WriteLine("```mermaid");
        writer.WriteLine("    graph");
        writer.WriteLine();
        foreach (var node in Nodes)
        {
            var (open, close) = node.Shape.Parenthesis();
            writer.WriteLine("{0}{1}{2}\"{3}\"{4}", INDENT, node.Key, open, node.Text, close);
        }
        var ix = 0;
        var indices = new List<int>();
        // A-. text .-> B
        foreach (var link in Links)
        {
            if (string.IsNullOrEmpty(link.Label))
            {
                writer.WriteLine("{0}{1}-->{2}", INDENT, link.SourceKey, link.TargetKey);
            }
            else
            {
                if (link.Label == "contains")
                {
                    writer.WriteLine("{0}{1}--{2}-->{3}", INDENT, link.SourceKey, link.Label, link.TargetKey);
                }
                else
                {
                    writer.WriteLine("{0}{1}-.{2}.->{3}", INDENT, link.SourceKey, link.Label, link.TargetKey);
                }
            }
            if (link.Label == "contains") { indices.Add(ix); }
            ix += 1;
        }
        // writer.WriteLine("    linkStyle {0} stroke:orange", string.Join(",", indices));
        writer.WriteLine("```");
    }
}