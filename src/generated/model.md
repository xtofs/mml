```mermaid
    graph

    n0("Schema: Edm")
    n1["PrimitiveType: Int32"]
    n2["PrimitiveType: String"]
    n3["PrimitiveType: Date"]
    n4("Schema: ODataDemo")
    n5["ComplexType: CompositeKey"]
    n6["StructuralProperty: ID"]
    n7["EntityType: Category"]
    n8["Key"]
    n9["PropertyRef"]
    n10["StructuralProperty: ID"]
    n11["StructuralProperty: Description"]
    n12["StructuralProperty: ReleaseDate"]
    n13["EntityType: Product"]
    n14["Key"]
    n15["PropertyRef"]
    n16["StructuralProperty: ID"]
    n17["StructuralProperty: Description"]
    n18["NavigationProperty: Category"]
    n0-->n1
    n0-->n2
    n0-->n3
    n4-->n5
    n4-->n7
    n4-->n13
    n5-->n6
    n6-.->|Type|n2
    n7-->n8
    n7-->n10
    n7-->n11
    n7-->n12
    n8-->n9
    n9-.->|Name|n6
    n10-.->|Type|n5
    n11-.->|Type|n2
    n12-.->|Type|n3
    n13-->n14
    n13-->n16
    n13-->n17
    n13-->n18
    n14-->n15
    n15-.->|Name|n16
    n16-.->|Type|n1
    n17-.->|Type|n2
    n18-.->|Type|n7
```
