namespace EDM;

using System;
using System.Collections.Generic;
using Graph;

sealed record Model : Node
{
    public Model() : base("$ModelRoot")
    {
        Schemas = new AdjacentNodes<Schema>(this, Graph.Link.Containment);
    }

    public AdjacentNodes<Schema> Schemas { get; }

    public mermaid.Diagram ToDiagram()
    {
        var nodes = Descendants(this).ToArray();

        var diagram = new mermaid.Diagram();
        foreach (var node in nodes)
        {
            diagram.AddNode(node.Name, $"{node.GetType().Name}: {node.Name}");
        }
        foreach (var (s, l, t) in
            from n in nodes
            from lnk in n.Links
            where lnk.Label.Direction
            select (n.Name, lnk.Label.Name, lnk.Target.Name))
        {
            diagram.AddLink(s, t, l);
        }
        return diagram;
    }


    private static IEnumerable<INode> Descendants(INode node)
    {
        foreach (var direct in node.Links
            .Where(lnk => lnk.Label == Link.Containment.Forward)
            .Select(lnk => lnk.Target))
        {
            yield return direct;
            foreach (var descendant in Descendants(direct))
            {
                yield return descendant;
            }
        }
    }
}


sealed record Schema : Node
{
    public Schema(string Namespace, string Alias) : base(Namespace)
    {
        Elements = new AdjacentNodes<SchemaElement>(this, Graph.Link.Containment);
    }

    public AdjacentNodes<SchemaElement> Elements { get; }
}

interface SchemaElement : INode { }

sealed record EnumType : Node, SchemaElement
{
    public EnumType(string Name) : base(Name)
    {
        Members = new AdjacentNodes<EnumMember>(this, Graph.Link.Containment);
    }

    public AdjacentNodes<EnumMember> Members { get; }
}

sealed record EnumMember(string Name, int? Value = null) : Graph.Node(Name);

sealed record ComplexType : Node, SchemaElement
{
    public ComplexType(string Name) : base(Name)
    {
        Properties = new AdjacentNodes<Property>(this, Graph.Link.Containment);
    }

    public AdjacentNodes<Property> Properties { get; }
}


sealed record Property : Graph.Node
{
    public Property(string name, Node type) : base(name)
    {
        this.LinkTo(type, Property.PropertyType);
    }
    public static Link PropertyType = ("PropertyType", "TypeOfProperty");

}

sealed record Primitive(string Name) : Graph.Node(Name)
{
    public static readonly Primitive Int32 = new Primitive("Int32");

}

