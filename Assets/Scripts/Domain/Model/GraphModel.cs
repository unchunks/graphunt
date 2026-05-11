using System;
using System.Collections.Generic;

public class GraphModel
{
    // グラフの実体
    private readonly Dictionary<int, NodeData> _nodes = new();
    private readonly HashSet<EdgeData> _edges = new();
    private readonly Dictionary<int, HashSet<int>> _adjacencyList = new();

    public ulong CurrentHash { get; set; }  // Commandが差分更新する

    // TODO: オオカミが2体にできるように仕様を変更する
    // プレイヤー現在地（0=ウサギ, 1=オオカミA, 2=オオカミB, インデックスは PlayerID にキャスト）
    private readonly int[] _playerPositions = new int[3];

    // イベント
    public event Action<PlayerID, int> OnPlayerMoved;   // (playerId, toNodeId)
    public event Action<EdgeData> OnEdgeRemoved;
    public event Action<EdgeData> OnEdgeAdded;
    public event Action<NodeData> OnNodeAdded;

    #region ノード操作

    /// <summary>ノードのデータとViewを追加する。隣接リストの空エントリも同時に作成。</summary>
    public void AddNode(NodeData node)
    {
        _nodes[node.Id] = node;

        if (!_adjacencyList.ContainsKey(node.Id))
        {
            _adjacencyList[node.Id] = new HashSet<int>();
        }

        OnNodeAdded?.Invoke(node);
    }

    #endregion

    #region エッジ操作

    /// <summary>エッジのデータとViewを追加する。_edges と _adjacencyList を同期して更新。</summary>
	public void AddEdge(EdgeData edge)
    {
        if (!_edges.Add(edge)) return;  // 重複は無視

        _adjacencyList[edge.NodeA].Add(edge.NodeB);
        _adjacencyList[edge.NodeB].Add(edge.NodeA);
        OnEdgeAdded?.Invoke(edge);
    }

    /// <summary>エッジのデータとViewを削除する。_edges と _adjacencyList を同期して更新。</summary>
    public void RemoveEdge(EdgeData edge)
    {
        if (!_edges.Remove(edge)) return;  // 存在しないエッジは無視

        _adjacencyList[edge.NodeA].Remove(edge.NodeB);
        _adjacencyList[edge.NodeB].Remove(edge.NodeA);
        OnEdgeRemoved?.Invoke(edge);
    }

    #endregion

    #region プレイヤー操作

    /// <summary>ステージ読み込み時の初期位置設定。イベントは発行しない。</summary>
    public void InitializePlayerPositions(int rabbitNodeId, int wolfANodeId, int wolfBNodeId)
    {
        _playerPositions[(int)PlayerID.Rabbit] = rabbitNodeId;
        _playerPositions[(int)PlayerID.WolfA] = wolfANodeId;
        _playerPositions[(int)PlayerID.WolfB] = wolfBNodeId;
    }

    /// <summary>ターン中の移動。イベントを発行する。</summary>
    public void MovePlayer(PlayerID playerId, int toNodeId)
    {
        _playerPositions[(int)playerId] = toNodeId;
        OnPlayerMoved?.Invoke(playerId, toNodeId);
    }

    public int GetPlayerPosition(PlayerID playerId)
        => _playerPositions[(int)playerId];

    #endregion

    #region 読み取り専用メンバへのアクセス

    public IReadOnlyDictionary<int, NodeData> Nodes => _nodes;
    public NodeType GetNodeType(int nodeId) => Nodes[nodeId].Type;
    public IReadOnlyCollection<EdgeData> Edges => _edges;

    /// <summary>
    /// 隣接ノードIDの読み取り専用コレクションを返す。
    /// </summary>
    public IReadOnlyCollection<int> GetNeighbors(int nodeId)
    {
        if (_adjacencyList.TryGetValue(nodeId, out var neighbors))
            return neighbors;

        return Array.Empty<int>();
    }

    /// <summary>2ノード間にエッジが存在するか。</summary>
    public bool HasEdge(int nodeA, int nodeB)
        => _adjacencyList.TryGetValue(nodeA, out var neighbors)
           && neighbors.Contains(nodeB);

    #endregion
}
