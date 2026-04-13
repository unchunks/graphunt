public class NodeData
{
    public int Id { get; }
    public NodeType Type { get; private set; }  // Normal / Goal

    public NodeData(int id, NodeType type = NodeType.Normal)
    {
        Id = id;
        Type = type;
    }

    public void SetType(NodeType type) => Type = type;

    public override string ToString() => $"Node({Id}, {Type})";
}

public enum NodeType
{
    Normal,
    Goal,
}
