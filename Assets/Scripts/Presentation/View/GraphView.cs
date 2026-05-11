using System.Collections.Generic;
using UnityEngine;

public class GraphView : MonoBehaviour
{
    [SerializeField] private NodeView _nodeViewPrefab;
    [SerializeField] private EdgeView _edgeViewPrefab;

    private GraphModel _graph;

    // NodeId → NodeView の対応表
    private readonly Dictionary<int, NodeView> _nodeViews = new();
    // EdgeData → EdgeView の対応表
    // TODO: int Hashをキーにしたい
    private readonly Dictionary<EdgeData, EdgeView> _edgeViews = new();

    public void Initialize(GraphModel graph)
    {
        _graph = graph;

        _graph.OnNodeAdded += HandleNodeAdded;
        _graph.OnEdgeAdded += HandleEdgeAdded;
        _graph.OnEdgeRemoved += HandleEdgeRemoved;

        foreach (var node in _graph.Nodes.Values)
            HandleNodeAdded(node);

        foreach (var edge in _graph.Edges)
            HandleEdgeAdded(edge);

        SmoothNodePosition();
    }

    private void OnDestroy()
    {
        if (_graph == null) return;

        _graph.OnNodeAdded -= HandleNodeAdded;
        _graph.OnEdgeAdded -= HandleEdgeAdded;
        _graph.OnEdgeRemoved -= HandleEdgeRemoved;
    }

    #region イベントハンドラ

    private void HandleNodeAdded(NodeData node)
    {
        if (_nodeViews.ContainsKey(node.Id)) return;

        NodeView view = Instantiate(_nodeViewPrefab, transform);
        view.name = $"Node_{node.Id}";
        Debug.Log(view.name);

        view.Initialize(node, _graph);
        _nodeViews[node.Id] = view;
    }

    private void HandleEdgeAdded(EdgeData edge)
    {
        if (_edgeViews.ContainsKey(edge)) return;

        // 両端の NodeView が存在することを確認
        if (!_nodeViews.TryGetValue(edge.NodeA, out NodeView viewA)) return;
        if (!_nodeViews.TryGetValue(edge.NodeB, out NodeView viewB)) return;

        EdgeView view = Instantiate(_edgeViewPrefab, transform);
        view.name = $"Edge_{edge.NodeA}_{edge.NodeB}";

        view.Initialize(edge, viewA, viewB);
        _edgeViews[edge] = view;
    }

    private void HandleEdgeRemoved(EdgeData edge)
    {
        if (!_edgeViews.TryGetValue(edge, out EdgeView view)) return;

        _edgeViews.Remove(edge);
        Destroy(view.gameObject);
    }

    #endregion

    #region Fruchterman-Reingold アルゴリズムによるレイアウト

    private readonly Dictionary<int, Vector2> _pos = new();
    private readonly Dictionary<int, Vector2> _disp = new();
    private float _k;
    private float _temperature;
    private const float EPS = 0.01f;

    void Update()
    {
        if (_nodeViews.Count == 0) return;
        if (!IsConverged())
        {
            Step();
        }
    }

    private void SmoothNodePosition()
    {
        Rect area = new Rect(0, 0, 10, 10);

        int n = _nodeViews.Count;
        float A = area.width * area.height;
        _k = Mathf.Sqrt(A / n);
        _temperature = area.width * 0.15f;

        InitializePositions(area);
    }

    private void InitializePositions(Rect area)
    {
        float radius = Mathf.Min(area.width, area.height) * 0.4f;
        int i = 0;
        foreach (var id in _nodeViews.Keys)
        {
            float angle = 2 * Mathf.PI * i / _nodeViews.Count;
            _pos[id] = new Vector2(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius
            );
            i++;
        }
    }

    public void Step(float cooling = 0.95f)
    {
        foreach (var id in _nodeViews.Keys)
            _disp[id] = Vector2.zero;

        // 斥力（全ノード対）
        var ids = new List<int>(_nodeViews.Keys);
        for (int i = 0; i < ids.Count; i++)
        {
            for (int j = i + 1; j < ids.Count; j++)
            {
                int v = ids[i];
                int u = ids[j];

                Vector2 delta = _pos[v] - _pos[u];
                float dist = Mathf.Max(EPS, delta.magnitude);
                Vector2 force = delta.normalized * (_k * _k / dist);

                _disp[v] += force;
                _disp[u] -= force;
            }
        }

        // 引力（エッジ）
        foreach (var edge in _edgeViews.Keys)
        {
            int v = edge.NodeA;
            int u = edge.NodeB;

            Vector2 delta = _pos[v] - _pos[u];
            float dist = Mathf.Max(EPS, delta.magnitude);
            Vector2 force = delta.normalized * (dist * dist / _k);

            _disp[v] -= force;
            _disp[u] += force;
        }

        // 位置更新
        foreach (var id in ids)
        {
            Vector2 d = _disp[id];
            float len = d.magnitude;
            if (len > EPS)
            {
                Vector2 move = d.normalized * Mathf.Min(len, _temperature);
                _pos[id] += move;

                // Unity の座標へ反映（XY平面）
                _nodeViews[id].transform.position =
                    new Vector3(_pos[id].x, 0f, _pos[id].y);
            }
        }

        _temperature *= cooling;

        // エッジの位置更新
        foreach (var pair in _edgeViews)
        {
            EdgeData edge = pair.Key;
            EdgeView view = pair.Value;

            Vector3 a = _nodeViews[edge.NodeA].transform.position;
            Vector3 b = _nodeViews[edge.NodeB].transform.position;

            view.UpdatePositions(a, b);
        }
    }


    public bool IsConverged(float threshold = 0.1f)
        => _temperature < threshold;

    #endregion

    public bool TryGetNodeView(int nodeId, out NodeView view)
        => _nodeViews.TryGetValue(nodeId, out view);
}
