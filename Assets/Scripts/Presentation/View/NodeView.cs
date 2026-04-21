using UnityEngine;

public class NodeView : MonoBehaviour
{
    public int NodeId { get; private set; }

    private NodeData _node;
    private Renderer _renderer;

    private static readonly Color ColorNormal = Color.gray;
    private static readonly Color ColorGoal = Color.yellow;

    public void Initialize(NodeData node)
    {
        _node = node;
        NodeId = node.Id;
        _renderer = GetComponent<Renderer>();

        UpdateColor();
    }

    private void UpdateColor()
    {
        _renderer.material.color = _node.Type == NodeType.Goal
            ? ColorGoal
            : ColorNormal;
    }
}
