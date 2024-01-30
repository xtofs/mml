using mml;

var path = "data/minimal.mml";
if (MetaModel.TryParse(File.ReadAllText(path), out var model))
{

    model.Display(Console.Out);
    var @out = Path.Combine("../generated", Path.ChangeExtension(Path.GetFileName(path), "generated.cs"));
    using var writer = File.CreateText(@out);
    model.GenerateCode(writer, path);
}
else
{
    Console.Error.WriteLine("failed to parse {0}", path);
}