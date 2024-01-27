using System.Reflection.Metadata.Ecma335;
using mermaid;

var a = new EnumType("Color");
var b = new EnumMember("Red", 1);
var c = new EnumMember("Green", 2);
a.Link(b, Label.ParentChild);
a.Link(c, Label.ParentChild);

try
{
    var b2 = new EnumMember("Red", 7);
    a.Link(b2, Label.ParentChild);
}
catch (System.ArgumentException)
{ }

if (a.TryFind<EnumMember>("Red", out var r))
{
    Console.WriteLine("r = {0}", r);
}

var diagram = Mermaid.FromNodes([a, b, c]);
diagram.WriteTo("demo.md");

public static class Mermaid
{
    public static Diagram FromNodes(params Node[] nodes)
    {
        var diagram = new Diagram();
        foreach (var node in nodes)
        {
            diagram.AddNode(node.Name, $"{node.GetType().Name}: {node.Name}");
        }
        foreach (var (s, t, l) in from n in nodes from lnk in n.Links select (n.Name, lnk.Target.Name, lnk.Label))
        {
            diagram.AddLink(s, t, l);
        }
        return diagram;
    }
}