

## ToDo
- make `StructuralProperty`  `Type` a relative path, raltive to its parent EntityType
    e.g. not 
    ```xml
    <EntityType Name="Product">
        <Key>
         <PropertyRef Name="ODataDemo.Product.ID" />
    ```
    but 
    ```xml
    <EntityType Name="Product">
        <Key>
         <PropertyRef Name="ID" />
    ```
- add configuration to map mml class names to XML element tags (e.g. StructuralProperty-> Property)

<PropertyRef Name="ODataDemo.CompositeKey/ID" />
<StructuralProperty Name="Description" Type="Edm.String" />

<PropertyRef Name="ID/ID" />
// path up to enclosing EntityType
// i.e. two levels up in the Parent/Child relationship
// EntityType|Key|PropertyRef

<StructuralProperty Name="Description" Type="Edm.String" />
// path up to enclosing model
// i.e. three levels up in the Parent/Child relationship
//   Model|Schema|StructuralType|StructuralProperty
