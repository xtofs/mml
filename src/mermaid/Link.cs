namespace mermaid;

public record Link(string SourceKey, string TargetKey, string Label)
{
    public void WriteTo(TextWriter writer)
    {

    }
}

