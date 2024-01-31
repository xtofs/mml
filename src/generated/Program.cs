using model;

// https://docs.oasis-open.org/odata/odata-csdl-xml/v4.01/odata-csdl-xml-v4.01.html#sec_ProductsandCategoriesExample

var model = new Model();

var edm = model.Nodes.Add(new Schema("EDM", "EDM"));
var EDM = new
{
    String = edm.Elements.Add(new PrimitiveType("String")),
    Int32 = edm.Elements.Add(new PrimitiveType("Int32"))
};


//       <EntityType Name="Product" HasStream="true">
//         <Key>
//           <PropertyRef Name="ID" />
//         </Key>
//         <Property Name="ID" Type="Edm.Int32" Nullable="false" />
//         <Property Name="Description" Type="Edm.String" />
//         <Property Name="ReleaseDate" Type="Edm.Date" />
//         <Property Name="DiscontinuedDate" Type="Edm.Date" />
//         <Property Name="Rating" Type="Edm.Int32" />
//         <Property Name="Price" Type="Edm.Decimal" Scale="variable"/>
//         <Property Name="Currency" Type="Edm.String" MaxLength="3" />
//         <NavigationProperty Name="Category" Type="ODataDemo.Category" Nullable="false" Partner="Products" />
//         <NavigationProperty Name="Supplier" Type="ODataDemo.Supplier" Partner="Products" />
//       </EntityType>

var schema = model.Nodes.Add(new Schema("ODataDemo", "self"));
var product = schema.Elements.Add(new EntityType("Product"));
var id = product.Properties.Add(new StructuralProperty("ID") { Type = EDM.Int32 });
var description = product.Properties.Add(new StructuralProperty("Description") { Type = EDM.String });

// var keys = product.Key.Set(new Key());
// var key = keys.PropertyRefs.Add(new PropertyRef("ID_ref") { Name = id });

Console.WriteLine(schema);

var diagram = model.ToDiagram();
diagram.WriteTo("model.md");