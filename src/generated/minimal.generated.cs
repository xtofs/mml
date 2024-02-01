// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and
//     will be lost if the code is regenerated.
// </auto-generated>
// generated from data/minimal.mml, written 2024-01-31 21::13:26

using model;
public class Schema: Node
{
    public  Schema (string Namespace, string Alias) : base(Namespace)
    {
        this.Namespace = Namespace;
        this.Alias = Alias;
        this.Elements = new ContainedCollection<SchemaElement>(this, (x) => x.Name);
    }

    public override string Tag { get; } = "Schema";

    public override IEnumerable<(string, object)> Attributes => [(nameof(Namespace), Namespace), (nameof(Alias), Alias)];

    public string Namespace { get; }

    public string Alias { get; }

    public ContainedCollection<SchemaElement> Elements { get; }
}

public interface SchemaElement: INode
{
}

public interface Type: INode
{
}

public interface ValueType: INode, Type
{
}

public class PrimitiveType: Node, SchemaElement, ValueType
{
    public  PrimitiveType (string Name) : base(Name)
    {
        this.Name = Name;
    }

    public override string Tag { get; } = "PrimitiveType";

    public override IEnumerable<(string, object)> Attributes => [(nameof(Name), Name)];

}

public class EnumType: Node, SchemaElement, ValueType
{
    public  EnumType (string Name) : base(Name)
    {
        this.Name = Name;
        this.Members = new ContainedCollection<Member>(this, (x) => x.Name);
    }

    public override string Tag { get; } = "EnumType";

    public override IEnumerable<(string, object)> Attributes => [(nameof(Name), Name)];


    public ContainedCollection<Member> Members { get; }
}

public class Member: Node
{
    public  Member (string Name, int Value) : base(Name)
    {
        this.Name = Name;
        this.Value = Value;
    }

    public override string Tag { get; } = "Member";

    public override IEnumerable<(string, object)> Attributes => [(nameof(Name), Name), (nameof(Value), Value)];


    public int Value { get; }
}

public class ComplexType: Node, SchemaElement, ValueType
{
    public  ComplexType (string Name) : base(Name)
    {
        this.Name = Name;
        this.Properties = new ContainedCollection<Property>(this, (x) => x.Name);
    }

    public override string Tag { get; } = "ComplexType";

    public override IEnumerable<(string, object)> Attributes => [(nameof(Name), Name)];


    public ContainedCollection<Property> Properties { get; }
}

public class EntityType: Node, SchemaElement, Type
{
    public  EntityType (string Name) : base(Name)
    {
        this.Name = Name;
        this._Key = new ContainedSingleton<Key>(this);
        this.Properties = new ContainedCollection<Property>(this, (x) => x.Name);
    }

    public override string Tag { get; } = "EntityType";

    public override IEnumerable<(string, object)> Attributes => [(nameof(Name), Name)];


    private ContainedSingleton<Key> _Key { get; }

    public Key Key { get => _Key.Get(); set => _Key.Set(value); }

    public ContainedCollection<Property> Properties { get; }
}

public class Key: Node
{
    public  Key () : base("")
    {
        this.PropertyRefs = new ContainedCollection<PropertyRef>(this, (x) => x.Alias);
    }

    public override string Tag { get; } = "Key";

    public override IEnumerable<(string, object)> Attributes => [];

    public ContainedCollection<PropertyRef> PropertyRefs { get; }
}

public class PropertyRef: Node
{
    public  PropertyRef (string Alias) : base(Alias)
    {
        this.Alias = Alias;
        this._Name = new ReferencedSingleton<Property>(this, "Name");
    }

    public override string Tag { get; } = "PropertyRef";

    public override IEnumerable<(string, object)> Attributes => [(nameof(Alias), Alias)];

    private ReferencedSingleton<Property> _Name { get; }

    public new Property Name { get => _Name.Get(); set => _Name.Set(value); }

    public string Alias { get; }
}

public interface Property: INode
{
}

public class StructuralProperty: Node, Property
{
    public  StructuralProperty (string Name) : base(Name)
    {
        this.Name = Name;
        this._Type = new ReferencedSingleton<ValueType>(this, "Type");
    }

    public override string Tag { get; } = "StructuralProperty";

    public override IEnumerable<(string, object)> Attributes => [(nameof(Name), Name)];


    private ReferencedSingleton<ValueType> _Type { get; }

    public ValueType Type { get => _Type.Get(); set => _Type.Set(value); }
}

public class NavigationProperty: Node, Property
{
    public  NavigationProperty (string Name, bool ContainsTarget) : base(Name)
    {
        this.Name = Name;
        this.ContainsTarget = ContainsTarget;
        this._Type = new ReferencedSingleton<EntityType>(this, "Type");
    }

    public override string Tag { get; } = "NavigationProperty";

    public override IEnumerable<(string, object)> Attributes => [(nameof(Name), Name), (nameof(ContainsTarget), ContainsTarget)];


    private ReferencedSingleton<EntityType> _Type { get; }

    public EntityType Type { get => _Type.Get(); set => _Type.Set(value); }

    public bool ContainsTarget { get; }
}

