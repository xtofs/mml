namespace model;

using mermaid;

public static class ModelDiagramExtensions
{


    public static mermaid.Diagram ToDiagram(this Model root)
    {
        var nodeLookup = root.Descendants().Enumerate().ToDictionary();
        var diagram = new mermaid.Diagram();

        // Nodes
        foreach (var (node, ix) in nodeLookup)
        {
            var shape = node.GetType() == typeof(Schema) ? NodeShape.RoundedBox : NodeShape.Box;
            var name = string.IsNullOrWhiteSpace(node.Name) ? node.GetType().Name : $"{node.GetType().Name}: {node.Name}";
            diagram.AddNode($"n{ix}", name, shape);
        }

        // Links
        int GetTargetId(INode target)
        {
            return nodeLookup.TryGetValue(target, out var tgt) ? tgt : throw new KeyNotFoundException($"Value for Key '{target}' not found");
        }

        var triples = from nodeAndIndex in nodeLookup
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
