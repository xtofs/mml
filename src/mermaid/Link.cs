namespace mermaid;

public enum Tip { None, Arrow, Circle, Cross }

public enum Line { Normal, Thick, Dotted }


public record class LinkStyle(Tip Left, Line Line, int Length, Tip Right)
{
    public string Format()
    {
        var (l, m, r) = (Left, Line, Right) switch
        {
            (Tip.None, Line.Normal, Tip.None) => ('-', '-', "-"),
            (Tip.None, Line.Thick, Tip.None) => ('=', '=', "="),
            (Tip.None, Line.Dotted, Tip.None) => ('-', '.', "-"),
            (Tip.None, Line.Normal, Tip.Arrow) => ('-', '-', ">"),
            (Tip.None, Line.Thick, Tip.Arrow) => ('=', '=', ">"),
            (Tip.None, Line.Dotted, Tip.Arrow) => ('-', '.', "->"),
            (Tip.None, Line.Normal, Tip.Circle) => ('-', '-', "o"),
            (Tip.None, Line.Thick, Tip.Circle) => ('=', '=', "o"),
            (Tip.None, Line.Dotted, Tip.Circle) => ('-', '.', "-o"),
            (Tip.None, Line.Normal, Tip.Cross) => ('-', '-', "x"),
            (Tip.None, Line.Thick, Tip.Cross) => ('=', '=', "x"),
            (Tip.None, Line.Dotted, Tip.Cross) => ('-', '.', "-x"),
        };
        return l + new string(m, Length) + r;
    }
}


public record Link(string SourceKey, string TargetKey, string Label, LinkStyle Style = null!)
{
    public LinkStyle Style { get; } = Style ?? DEFAULT;

    private static readonly LinkStyle DEFAULT = new LinkStyle(Tip.None, Line.Normal, 1, Tip.Arrow);

    public void WriteTo(TextWriter writer)
    {
        if (string.IsNullOrWhiteSpace(Label))
        {
            writer.WriteLine("    {0}{1}{2}", SourceKey, Style.Format(), TargetKey);
        }
        else
        {
            writer.WriteLine("    {0}{1}|{2}|{3}", SourceKey, Style.Format(), Label, TargetKey);
        }
    }
}

