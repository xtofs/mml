using mml;


if (MetaModel.TryParse(File.ReadAllText("data/example.mml"), out var model))
{

    model.Display(Console.Out);
}

