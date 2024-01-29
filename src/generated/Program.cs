using model;

var model = new Model();

var edm = model.Nodes.Add(new Schema("EDM", "EDM"));
var EDM = new { String = edm.Elements.Add(new PrimitiveType("String")) };

var schema = model.Nodes.Add(new Schema("example.org", "self"));
var color = schema.Elements.Add(new EnumType("Color"));
var _r = color.Members.Add(new Member("red", 1));
var _g = color.Members.Add(new Member("green", 2));
var _b = color.Members.Add(new Member("blue", 3));

var car = schema.Elements.Add(new ComplexType("Car"));
car.Properties.Add(new StructuralProperty("make") { Type = EDM.String });
car.Properties.Add(new StructuralProperty("model") { Type = EDM.String });
car.Properties.Add(new StructuralProperty("color") { Type = color });

Console.WriteLine(schema);

var diagram = model.ToDiagram();
diagram.WriteTo("example_model.md");