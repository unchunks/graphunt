using System;

public class EdgeData : IEquatable<EdgeData>
{
    public int NodeA { get; }
    public int NodeB { get; }

    public EdgeData(int nodeId1, int nodeId2)
    {
        // 常に小さいIDをNodeAに格納し、順序を正規化する
        NodeA = Math.Min(nodeId1, nodeId2);
        NodeB = Math.Max(nodeId1, nodeId2);
    }

    public bool Connects(int nodeId) => NodeA == nodeId || NodeB == nodeId;
    public int Other(int nodeId) => NodeA == nodeId ? NodeB : NodeA;

    public bool Equals(EdgeData other)
    {
        if (other is null) return false;
        return NodeA == other.NodeA && NodeB == other.NodeB;
    }

    public override bool Equals(object obj) => Equals(obj as EdgeData);
    public override int GetHashCode() => HashCode.Combine(NodeA, NodeB);
    public override string ToString() => $"Edge({NodeA}-{NodeB})";
}
