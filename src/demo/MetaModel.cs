using edmml;

abstract record Classifier(string Name, IReadOnlyList<Field> Fields)
{
    public LineInfo LineInfo { get; internal init; }

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.AppendFormat("Name = {0}", Name);
        builder.AppendFormat(", Fields = [{0}]", string.Join(", ", Fields));
        // builder.AppendFormat(", LineInfo = {0}", LineInfo);
        return true;
    }
}


sealed record Class(string Name, IReadOnlyList<Field> Fields) : Classifier(Name, Fields)
{
    protected override bool PrintMembers(StringBuilder builder) => base.PrintMembers(builder);
}

sealed record Trait(string Name, IReadOnlyList<Field> Fields) : Classifier(Name, Fields)
{
    protected override bool PrintMembers(StringBuilder builder) => base.PrintMembers(builder);
}

sealed record Field(string Name, FieldType Type, LineInfo Position);


abstract record FieldType();

sealed record Builtin(string Name) : FieldType()
{
    public static Builtin String { get; } = new Builtin("string");
    public static Builtin Int { get; } = new Builtin("int");
    public static Builtin Bool { get; } = new Builtin("bool");

    public override string ToString() => $"{{Type {Name}}}";
}

sealed record Contained(string Name) : FieldType()
{
    public override string ToString() => $"{{Type {Name}}}";

}

sealed record Reference(string Name) : FieldType()
{
    public override string ToString() => $"{{Type &{Name}}}";
}
