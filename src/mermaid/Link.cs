namespace mermaid;

public record Link(string SourceKey, string TargetKey, string Label)
{
    public void WriteTo(TextWriter writer)
    {
        if (string.IsNullOrEmpty(Label))
        {
            writer.WriteLine("{0}{1}-->{3}", Node.INDENT, SourceKey, "-->", TargetKey);
        }
        else
        {
            writer.WriteLine("{0}{1}--{2}-->{3}", Node.INDENT, SourceKey, Label, TargetKey);
        }
    }
}

