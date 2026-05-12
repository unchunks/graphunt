using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GraphView : MonoBehaviour
{
    [SerializeField] private NodeView _nodeViewPrefab;
    [SerializeField] private EdgeView _edgeViewPrefab;

    [SerializeField] private GraphLayoutAnimator _animator;

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

        Debug.Log("GraphView initialized with " + _nodeViews.Count + " nodes and " + _edgeViews.Count + " edges.");
        CalculateAndAnimateLayout();
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

        //CalculateAndAnimateLayout();
    }

    private void HandleEdgeAdded(EdgeData edge)
    {
        if (_edgeViews.ContainsKey(edge)) return;

        // 両端の NodeView が存在することを確認
        if (!_nodeViews.TryGetValue(edge.NodeA, out NodeView viewA)) return;
        if (!_nodeViews.TryGetValue(edge.NodeB, out NodeView viewB)) return;

        EdgeView view = Instantiate(_edgeViewPrefab, transform);
        view.name = $"Edge_{edge.NodeA}_{edge.NodeB}";
        Debug.Log(view.name);

        view.Initialize(edge, viewA, viewB);
        _edgeViews[edge] = view;

        //CalculateAndAnimateLayout();
    }

    private void HandleEdgeRemoved(EdgeData edge)
    {
        if (!_edgeViews.TryGetValue(edge, out EdgeView view)) return;

        _edgeViews.Remove(edge);
        Destroy(view.gameObject);

        //CalculateAndAnimateLayout();
    }

    #endregion

    #region レイアウト計算とアニメーション

    private void CalculateAndAnimateLayout()
    {
        var ids = new List<int>(_nodeViews.Keys);
        var edges = new List<(int, int)>();

        foreach (var e in _edgeViews.Keys)
            edges.Add((e.NodeA, e.NodeB));

        var result = FruchtermanReingold.Solve(
            ids: _graph.Nodes.Keys.ToList(),
            edges: edges,
            radius: 5f,
            maxIterations: 500
        );

        var targets = new Dictionary<int, Vector3>();
        foreach (var kv in result)
            targets[kv.Key] = kv.Value;

        _animator.Initialize(_nodeViews, UpdateEdges);
        _animator.Play(targets);
    }

    private void UpdateEdges()
    {
        foreach (var pair in _edgeViews)
        {
            var edge = pair.Key;
            var view = pair.Value;

            Vector3 a = _nodeViews[edge.NodeA].transform.position;
            Vector3 b = _nodeViews[edge.NodeB].transform.position;

            view.UpdatePositions(a, b);
        }
    }

    #endregion

    public bool TryGetNodeView(int nodeId, out NodeView view)
        => _nodeViews.TryGetValue(nodeId, out view);
}
