using mml;

var path = "data/example.mml";
if (MetaModel.TryParse(File.ReadAllText(path), out var model))
{

    model.Display(Console.Out);

    using var writer = File.CreateText("../generated/Model.generated.cs");
    model.GenerateCode(writer);
}
else
{
    Console.Error.WriteLine("failed to parse {0}", path);
}