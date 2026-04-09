using System.Collections.Generic;

public class RuleEngine
{
    #region 公開メソッド
    /// <summary>隣接ノードへの移動が可能か。</summary>
    public bool CanMove(GraphModel g, PlayerID playerId, int targetNode)
    {
        int current = g.GetPlayerPosition(playerId);
        return g.HasEdge(current, targetNode);
    }

    /// <summary>エッジの切断が可能か（エッジが存在し、かつ橋でない）。</summary>
    public bool CanDisconnect(GraphModel g, int nodeA, int nodeB)
    {
        if (!g.HasEdge(nodeA, nodeB)) return false; // エッジの存在確認

        var bridges = FindBridges(g);
        int a = System.Math.Min(nodeA, nodeB);
        int b = System.Math.Max(nodeA, nodeB);
        return !bridges.Contains((a, b)); // 橋でないことの確認
    }
    /// <summary>現在地から距離2のノードへの接続が可能か。</summary>
    public bool CanConnect(GraphModel g, PlayerID playerId, int targetNode)
    {
        int current = g.GetPlayerPosition(playerId);

        // 既にエッジが存在する場合は生成不可
        if (g.HasEdge(current, targetNode)) return false;

        return GetDistance(g, current, targetNode) == 2;
    }

    /// <summary>BFSで最短距離を返す。連結グラフ前提のため -1 は返らない。</summary>
    public int GetGoalDistance(GraphModel g, int from, int goalNode)
        => GetDistance(g, from, goalNode);
    #endregion

    #region 内部メソッド
    /// <summary>
    /// LowLink法で橋を列挙する。O(V+E)。
    /// 橋は (小さいID, 大きいID) のタプルで正規化して返す。
    /// </summary>
    private HashSet<(int, int)> FindBridges(GraphModel g)
    {
        Dictionary<int, int> ord = new();
        Dictionary<int, int> low = new();
        HashSet<(int, int)> bridges = new();
        int timer = 0;

        foreach (int startNode in g.Nodes.Keys)
        {
            if (!ord.ContainsKey(startNode))
            {
                Dfs(startNode, -1);
            }
        }

        return bridges;
        
        void Dfs(int v, int parent)
        {
            ord[v] = low[v] = timer++;

            foreach (int neighbor in g.GetNeighbors(v))
            {
                if (!ord.ContainsKey(neighbor))
                {
                    // 未訪問 → 木辺
                    Dfs(neighbor, v);

                    // 子のlowを自分に伝播
                    low[v] = System.Math.Min(low[v], low[neighbor]);

                    // 橋の判定：子が自分より上に戻れない
                    if (low[neighbor] > ord[v])
                    {
                        int a = System.Math.Min(v, neighbor);
                        int b = System.Math.Max(v, neighbor);
                        bridges.Add((a, b));
                    }
                }
                else if (neighbor != parent)
                {
                    // 後退辺（親以外への辺）
                    low[v] = System.Math.Min(low[v], ord[neighbor]);
                }
            }
        }
    }

    /// <summary>
    /// BFSで from → to の最短距離を返す。O(V+E)。
    /// </summary>
    private int GetDistance(GraphModel g, int from, int to)
    {
        if (from == to) return 0;

        HashSet<int> visited = new HashSet<int> { from };
        Queue<int> queue = new Queue<int>();
        queue.Enqueue(from);
        int distance = 0;
        
        while (queue.Count > 0)
        {
            // 現在の階層（同じ距離にあるノード）の数
            int levelSize = queue.Count;
            distance++;

            for (int i = 0; i < levelSize; i++)
            {
                int current = queue.Dequeue();

                foreach (int neighbor in g.GetNeighbors(current))
                {
                    if (neighbor == to) return distance;

                    if (visited.Add(neighbor))
                    {
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }

        return -1;
    }
    #endregion
}
