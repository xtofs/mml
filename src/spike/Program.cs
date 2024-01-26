// See https://aka.ms/new-console-template for more information




var a = new EnumType("Color");
var b = new EnumMember("Red", 1);
var c = new EnumMember("Green", 2);
a.Link(b, Label.Containment);
a.Link(c, Label.Containment);

try
{
    var b2 = new EnumMember("Red", 7);
    a.Link(b2, Label.Containment);
}
catch (System.ArgumentException)
{ }

Console.WriteLine(a);
Console.WriteLine(b);
Console.WriteLine(c);

if (a.TryFind<EnumMember>("Red", out var r))
{
    Console.WriteLine("r={0}", r);
}


sealed record EnumType(string Name) : IndexedNode(Name)
{
}

sealed record EnumMember(string Name, int? Value = null) : Node(Name);
