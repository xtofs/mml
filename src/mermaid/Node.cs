namespace mermaid;


// https://mermaid.js.org/syntax/flowchart.html

public record Node(string Key, string Text, NodeShape Shape = NodeShape.Box)
{
    public void WriteTo(TextWriter writer)
    {
        var (open, close) = this.Shape.Parenthesis();
        writer.WriteLine("{0}{1}{2}{3}{4}", INDENT, Key, open, Text, close);
    }

    internal const string INDENT = "    ";
}

public enum NodeShape
{
    Box,
    RoundedBox,
    StadiumShaped,
    Subroutine,
    Cylindrical,
    Database = Cylindrical,
    Circular,
    Asymmetric,
    Rombic,
    Hexagonal,
    Parallelogram,
    ParallelogramAlt,
    Trapezoid,
    TrapezoidAlt,
    DoubleCircle
}

public static class NodeShapeExtensions
{
    public static (string, string) Parenthesis(this NodeShape shape) => shape switch
    {
        NodeShape.Box => ("[", "]"),
        NodeShape.RoundedBox => ("(", ")"),
        NodeShape.StadiumShaped => ("([", "])"),
        NodeShape.Subroutine => ("[[", "]]"),
        NodeShape.Cylindrical => ("[(", ")]"),
        NodeShape.Circular => ("((", "))"),
        NodeShape.Asymmetric => (">", "]"),
        NodeShape.Rombic => ("{", "}"),
        NodeShape.Hexagonal => ("{{", "}}"),
        NodeShape.Parallelogram => ("[/", "/]"),
        NodeShape.ParallelogramAlt => ("[\\", "\\]"),
        NodeShape.Trapezoid => ("[\\", "/]"),
        NodeShape.TrapezoidAlt => ("[/", "\\]"),
        NodeShape.DoubleCircle => ("(((", ")))"),
        _ => ("?", "?")
    };
}