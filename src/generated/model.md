```mermaid
    graph

    n0("Schema: EDM")
    n1["PrimitiveType: Int32"]
    n2["PrimitiveType: String"]
    n3["PrimitiveType: Date"]
    n4("Schema: ODataDemo")
    n5["ComplexType: CompositeKey"]
    n6["StructuralProperty: ID"]
    n7["EntityType: Category"]
    n8["Key: "]
    n9["StructuralProperty: ID"]
    n10["StructuralProperty: Description"]
    n11["StructuralProperty: ReleaseDate"]
    n12["EntityType: Product"]
    n13["Key: "]
    n14["StructuralProperty: ID"]
    n15["StructuralProperty: Description"]
    n16["NavigationProperty: Category"]
    n0-->n1
    n0-->n2
    n0-->n3
    n4-->n5
    n4-->n7
    n4-->n12
    n5-->n6
    n6-.->|Type|n2
    n7-->n8
    n7-->n9
    n7-->n10
    n7-->n11
    n9-.->|Type|n5
    n10-.->|Type|n2
    n11-.->|Type|n3
    n12-->n13
    n12-->n14
    n12-->n15
    n12-->n16
    n14-.->|Type|n1
    n15-.->|Type|n2
    n16-.->|Type|n7
```
