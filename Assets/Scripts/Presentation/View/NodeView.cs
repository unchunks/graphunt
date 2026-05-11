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

    private void HandlePlayerMoved(PlayerID playerId, int toNodeId)
    {
        UpdateColor();
    }

    private void UpdateColor()
    {
        int rabbitNode = _graph.GetPlayerPosition(PlayerID.Rabbit);
        int wolfANode = _graph.GetPlayerPosition(PlayerID.WolfA);
        int wolfBNode = _graph.GetPlayerPosition(PlayerID.WolfB);

        if (NodeId == rabbitNode && (NodeId == wolfANode || NodeId == wolfBNode))
        {
            // 同じノードにいる場合（捕獲時）は赤を優先
            _renderer.material.color = ColorWolf;
        }
        else if (NodeId == rabbitNode)
        {
            _renderer.material.color = ColorRabbit;
        }
        else if (NodeId == wolfANode || NodeId == wolfBNode)
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
