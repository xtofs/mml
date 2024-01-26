using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using edmml;


// var input = """
//     class Schema { 
//         Namespace: string,
//         Alias: string,
//         Elements: Dictionary<SchemaElement.Name>,
//     }

//     trait SchemaElement { // any of the elements of a schema
//          Name: string,
//     }
//     class EntityType extends SchemaElement

//     """;

var input = """
    class Schema { 
        Namespace: string,
        Alias: string,
        Elements: Dictionary<SchemaElement.Name>
    }

    trait SchemaElement { 
        Name: string,
    }

    class EntityType 
    {
    }

    """;

foreach (var token in Scanner.Scan(input))
{
    Console.WriteLine(token);
}
Console.WriteLine();


var tokens = Scanner.Scan(input, false).ToArray().AsMemory();

if (MetaModelParser.Classifiers(tokens, out var res))
{
    System.Console.WriteLine("Success: {0}", res.Value);

    var w = new IndentedTextWriter(Console.Out);
    foreach (var x in res.Value)
    {
        x.Display(w);

    }
}
