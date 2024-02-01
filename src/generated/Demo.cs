using System.Xml;
using model;

// https://docs.oasis-open.org/odata/odata-csdl-xml/v4.01/odata-csdl-xml-v4.01.html#sec_ProductsandCategoriesExample

var model = new Model();

#region edm
var edm = model.Nodes.Add(new Schema("Edm", "Edm"));
var EDM = new
{
    Int32 = edm.Elements.Add(new PrimitiveType("Int32")),
    String = edm.Elements.Add(new PrimitiveType("String")),
    Date = edm.Elements.Add(new PrimitiveType("Date")),
};
#endregion;

var schema = model.Nodes.Add(new Schema("ODataDemo", "self"));

var compositeKey = schema.Elements.Add(new ComplexType("CompositeKey"));
var compositeKeyId = compositeKey.Properties.Add(new StructuralProperty("ID") { Type = EDM.String });

var category = schema.Elements.Add(new EntityType("Category"));

category.Properties.Add(new StructuralProperty("ID") { Type = compositeKey });
category.Properties.Add(new StructuralProperty("Description") { Type = EDM.String });
category.Properties.Add(new StructuralProperty("ReleaseDate") { Type = EDM.Date });

category.Key.PropertyRefs.Add(new PropertyRef(null) { Name = compositeKeyId });

var product = schema.Elements.Add(new EntityType("Product"));
var prodId = product.Properties.Add(new StructuralProperty("ID") { Type = EDM.Int32 });
product.Properties.Add(new StructuralProperty("Description") { Type = EDM.String });
product.Properties.Add(new NavigationProperty("Category", false) { Type = category });
product.Key.PropertyRefs.Add(new PropertyRef(null) { Name = prodId });

model.WriteXml(schema, Console.Out);

var diagram = model.ToDiagram();
diagram.WriteTo("model.md");
Console.WriteLine();
Console.WriteLine("wrote ./model.md(1,1)");

