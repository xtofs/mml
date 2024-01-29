using EDM;

var model = new Model();
var schema = model.Schemas.Add(new Schema("example.org", "self"));

var color = schema.Elements.Add(new EnumType("Color"));
var r = new EnumMember("Red", 1);
var g = new EnumMember("Green", 2);
var b = new EnumMember("Blue", 2);
color.Members.Add(r);
color.Members.Add(g);
color.Members.Add(b);

var car = schema.Elements.Add(new ComplexType("Car"));
var colorProp = car.Properties.Add(new Property("color", color));
var nameProp = car.Properties.Add(new Property("name", Primitive.Int32));


if (color.TryFind<EnumMember>("Red", out var r2))
{
    Console.WriteLine("found  {0}", r2);
}

var diagram = model.ToDiagram();
diagram.WriteTo("demo.md");

