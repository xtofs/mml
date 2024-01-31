using System.Xml;
using model;

// https://docs.oasis-open.org/odata/odata-csdl-xml/v4.01/odata-csdl-xml-v4.01.html#sec_ProductsandCategoriesExample

var model = new Model();

#region edm
var edm = model.Nodes.Add(new Schema("EDM", "EDM"));
var EDM = new
{
    Int32 = edm.Elements.Add(new PrimitiveType("Int32")),
    String = edm.Elements.Add(new PrimitiveType("String")),
    Date = edm.Elements.Add(new PrimitiveType("Date")),
};
#endregion;

var schema = model.Nodes.Add(new Schema("ODataDemo", "self"));

var category = schema.Elements.Add(new EntityType("Category"));
category.Properties.Add(new StructuralProperty("ID") { Type = EDM.Int32 });
category.Properties.Add(new StructuralProperty("Description") { Type = EDM.String });
category.Properties.Add(new StructuralProperty("ReleaseDate") { Type = EDM.Date });

var product = schema.Elements.Add(new EntityType("Product"));
product.Properties.Add(new StructuralProperty("ID") { Type = EDM.Int32 });
product.Properties.Add(new StructuralProperty("Description") { Type = EDM.String });
product.Properties.Add(new NavigationProperty("Category") { Type = category });

var diagram = model.ToDiagram();
diagram.WriteTo("model.md");

schema.WriteXml(model, Console.Out);
