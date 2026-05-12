using UnityEngine;

public class NodeView : MonoBehaviour
{
    public int NodeId { get; private set; }

    [Header("Emission Settings")]
    [SerializeField] private float _emissionIntensity = 2.0f; // 発光の強さ

    private NodeData _node;
    private Renderer _renderer;
    private Material _material;
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
        // マテリアルをインスタンス化してキャッシュ（パフォーマンスとメモリ対策）
        _material = _renderer.material;

        _graph.OnPlayerMoved += HandlePlayerMoved;

        UpdateColor();
    }

    private void OnDestroy()
    {
        if (_graph != null)
            _graph.OnPlayerMoved -= HandlePlayerMoved;

        if (_material != null)
            Destroy(_material);
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

        Color targetColor;

        if (NodeId == rabbitNode && (NodeId == wolfANode || NodeId == wolfBNode))
        {
            // 同じノードにいる場合（捕獲時）は赤を優先
            targetColor = ColorWolf;
        }
        else if (NodeId == rabbitNode)
        {
            targetColor = ColorRabbit;
        }
        else if (NodeId == wolfANode || NodeId == wolfBNode)
        {
            targetColor = ColorWolf;
        }
        else if (_node.Type == NodeType.Goal)
        {
            targetColor = ColorGoal;
        }
        else
        {
            targetColor = ColorNormal;
        }

        SetMaterialColor(targetColor);
    }

    /// <summary>
    /// ベースカラーと発光カラーを同時に設定する
    /// </summary>
    private void SetMaterialColor(Color color)
    {
        // ベースカラーの設定
        _material.color = color;

        // 色に強さ(Intensity)を掛けて発光させる（HDRカラーとして適用）
        _material.SetColor("_EmissionColor", color * _emissionIntensity);
    }
}
