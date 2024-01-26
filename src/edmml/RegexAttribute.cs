namespace edmml;

[AttributeUsage(AttributeTargets.Field)]
internal class RegexAttribute(string Pattern) : Attribute
{
    public string Pattern { get; } = Pattern;
}
