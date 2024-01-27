sealed record EnumType : IndexedNode
{
    public EnumType(string Name) : base(Name)
    {
        Members = new AdjacentNodesView<EnumMember>(this, Label.ParentChild);
    }

    public AdjacentNodesView<EnumMember> Members { get; }
}

sealed record EnumMember(string Name, int? Value = null) : Node(Name);
