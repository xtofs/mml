

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
- 

