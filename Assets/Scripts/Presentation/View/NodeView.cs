using UnityEngine;

public class NodeView : MonoBehaviour
{
    public int NodeId { get; private set; }

    private NodeData _node;
    private Renderer _renderer;
    private GraphModel _graph;

    private static readonly Color ColorNormal = Color.gray;
    private static readonly Color ColorGoal = Color.yellow;
    private static readonly Color ColorRabbit = Color.green;
    private static readonly Color ColorWolf = Color.red;

    public void Initialize(NodeData node, GraphModel graph)
    {
        _node = node;
        NodeId = node.Id;
        _graph = graph;
        _renderer = GetComponent<Renderer>();

        _graph.OnPlayerMoved += HandlePlayerMoved;

        UpdateColor();
    }

    private void OnDestroy()
    {
        if (_graph != null)
            _graph.OnPlayerMoved -= HandlePlayerMoved;
    }

    private void HandlePlayerMoved(PlayerType playerType, int toNodeId)
    {
        UpdateColor();
    }

    private void UpdateColor()
    {
        int rabbitNode = _graph.GetPlayerPosition(PlayerType.Rabbit);
        int wolfNode = _graph.GetPlayerPosition(PlayerType.Wolf);

        if (NodeId == rabbitNode && NodeId == wolfNode)
        {
            // 同じノードにいる（捕獲直前）場合は赤を優先
            _renderer.material.color = ColorWolf;
        }
        else if (NodeId == rabbitNode)
        {
            _renderer.material.color = ColorRabbit;
        }
        else if (NodeId == wolfNode)
        {
            _renderer.material.color = ColorWolf;
        }
        else if (_node.Type == NodeType.Goal)
        {
            _renderer.material.color = ColorGoal;
        }
        else
        {
            _renderer.material.color = ColorNormal;
        }
    }
}
