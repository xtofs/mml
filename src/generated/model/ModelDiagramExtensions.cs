namespace model;

using mermaid;

public static class ModelDiagramExtensions
{
    public static mermaid.Diagram ToDiagram(this Model root)
    {
        var nodes = root.Descendants().Enumerate().ToDictionary();

        var diagram = new mermaid.Diagram();
        foreach (var (node, ix) in nodes)
        {
            var shape = node.GetType() == typeof(Schema) ? NodeShape.RoundedBox : NodeShape.Box;
            diagram.AddNode($"n{ix}", $"{node.GetType().Name}: {node.Name}", shape);
        }

        int GetTargetId(INode target)
        {
            return nodes.TryGetValue(target, out var tgt) ? tgt : throw new KeyNotFoundException($"Value for Key '{target}' not found");
        }

        var triples = from nodeAndIndex in nodes
                      let node = nodeAndIndex.Key
                      let ix = nodeAndIndex.Value
                      from lnk in node.Links
                      where lnk.Target != root
                      select (ix, lnk.Label, GetTargetId(lnk.Target));
        foreach (var (source, label, target) in triples)
        {
            if (label == Label.CONTAINS)
            {
                diagram.AddLink($"n{source}", $"n{target}", "", ContainmentStyle);
            }
            else if (label == Label.CONTAINED)
            {
                // no link added since it is the inverse of CONTAINS
            }
            else
            {
                diagram.AddLink($"n{source}", $"n{target}", label, ReferenceStyle);
            }
        }
        return diagram;
    }

    static readonly LinkStyle ContainmentStyle = new LinkStyle(Tip.None, Line.Normal, 1, Tip.Arrow);
    static readonly LinkStyle ReferenceStyle = new LinkStyle(Tip.None, Line.Dotted, 1, Tip.Arrow);
}
