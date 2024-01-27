
namespace mml;

using System.Diagnostics.CodeAnalysis;
using mml.parsing;

public record MetaModel(IReadOnlyList<Classifier> Classifiers) : IParsable<MetaModel>
{
    public static MetaModel Parse(string s, IFormatProvider? provider)
    {
        if (TryParse(s, out var mm))
        {
            return mm;
        }
        return new MetaModel([]);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out MetaModel result)
    {
        return TryParse(s, null, out result);
    }

    public static bool TryParse([NotNullWhen(true)] string? input, IFormatProvider? provider, [MaybeNullWhen(false)] out MetaModel result)
    {
        var tokens = Scanner.Scan(input ?? "", false).ToArray().AsMemory();

        if (MetaModelParser.Classifiers(tokens, out var res))
        {
            result = new MetaModel(res.Value);
            return true;
        }

        result = null;
        return false;
    }

    public static MetaModel FromFile(string path)
    {
        var input = File.ReadAllText(path);
        if (MetaModel.TryParse(input, out var res))
        {
            return res;
        }
        else
        {
            return new MetaModel([]);
        }
    }

    public void Display(TextWriter @out)
    {
        var w = new IndentedTextWriter(@out);
        foreach (var x in this.Classifiers)
        {
            x.Display(w);
        }
    }
}

public abstract record Classifier(string Name, IReadOnlyList<Field> Fields, IReadOnlyList<string> Extends)
{
    public LineInfo LineInfo { get; internal init; }

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.AppendFormat("Name = {0}", Name);
        builder.AppendFormat(", Fields = [{0}]", string.Join(", ", Fields));
        builder.AppendFormat(", Extends = [{0}]", string.Join(", ", Extends));
        // builder.AppendFormat(", LineInfo = {0}", LineInfo);
        return true;
    }

    protected abstract string Kind { get; }

    public void Display(IndentedTextWriter writer = null!)
    {
        writer ??= new IndentedTextWriter(Console.Out);

        writer.Write($"{Kind} {Name}");
        if (Extends.Any())
        {
            writer.Write($" extends {string.Join(" + ", Extends)}");
        }
        if (Fields.Any())
        {
            writer.WriteLine(" {");
            var w = Fields.Max(f => f.Name.Length);
            writer.Indent += 1;
            foreach (var (field, last) in Fields.WithLast())
            {
                field.Display(writer, w);
                if (!last) { writer.Write(", "); }
                writer.WriteLine();
            }
            writer.Indent -= 1;
            writer.WriteLine($"}}");
        }
        else
        {
            writer.WriteLine(" { }");
        }
        writer.WriteLine();
    }
}

public sealed record Class(string Name, IReadOnlyList<Field> Fields, IReadOnlyList<string> Extends) :
    Classifier(Name, Fields, Extends)
{
    protected override string Kind => "class";

    protected override bool PrintMembers(StringBuilder builder) => base.PrintMembers(builder);
}

public sealed record Trait(string Name, IReadOnlyList<Field> Fields, IReadOnlyList<string> Extends) :
    Classifier(Name, Fields, Extends)
{
    protected override bool PrintMembers(StringBuilder builder) => base.PrintMembers(builder);

    protected override string Kind => "trait";
}

public sealed record Field(string Name, FieldType Type, LineInfo Position)
{
    internal void Display(IndentedTextWriter writer, int w)
    {
        writer.Write($"{(Name + ':').PadRight(w + 1)} ");
        Type.Display(writer);
    }
}

public abstract record FieldType()
{
    public abstract void Display(IndentedTextWriter writer);
}

public sealed record Builtin(string Name) : FieldType()
{
    public static Builtin String { get; } = new Builtin("string");
    public static Builtin Int { get; } = new Builtin("int");
    public static Builtin Bool { get; } = new Builtin("bool");

    public override string ToString() => $"{{Type {Name}}}";

    public override void Display(IndentedTextWriter writer)
    {
        writer.Write(Name);
    }
}

public sealed record Contained(string Name) : FieldType()
{
    public override string ToString() => $"{{Type {Name}}}";

    public override void Display(IndentedTextWriter writer)
    {
        writer.Write(Name);
    }
}

sealed record Reference(string Name) : FieldType()
{
    public override string ToString() => $"{{Type &{Name}}}";

    public override void Display(IndentedTextWriter writer)
    {
        writer.Write("&{0}", Name);
    }
}

sealed record Dictionary(string Type, IReadOnlyList<string> Path) : FieldType()
{
    public override string ToString() => $"{{Type &{string.Join(".", Path)}}}";

    public override void Display(IndentedTextWriter writer)
    {
        // writer.Write("Dictionary<{0}>", string.Join(".", Path));
        writer.Write("[{0} | {1}]", Type, string.Join(".", Path));
    }
}
