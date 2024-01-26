using edmml;

var input = File.ReadAllText("example.edmml");

// foreach (var token in Scanner.Scan(input))
// {
//     Console.WriteLine(token);
// }
// Console.WriteLine();


var tokens = Scanner.Scan(input, false).ToArray().AsMemory();

if (MetaModelParser.Classifiers(tokens, out var res))
{
    Console.WriteLine("Success: ");
    // foreach (var x in res.Value)
    // {
    //     Console.WriteLine("    {0}", x);
    // }

    var w = new IndentedTextWriter(Console.Out);
    foreach (var x in res.Value)
    {
        x.Display(w);
    }
}
