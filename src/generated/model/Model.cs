namespace model;

public class Model : Node
{
    public Model() : base("$MODEL_ROOT")
    {
        this.Nodes = new ContainedCollection<Node>(this, (x) => x.Name);
    }

    public ContainedCollection<Node> Nodes { get; }

    public override IEnumerable<(string, object)> Attributes => [];

    public override string NodeTag => "Model";
}
