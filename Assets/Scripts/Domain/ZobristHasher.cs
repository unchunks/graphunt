using System;

public class ZobristHasher
{
    private readonly ulong[] _rabbitHashes;  // ウサギがノードiにいる時
    private readonly ulong[] _wolfHashes;    // オオカミがノードiにいる時（オオカミAとBは区別しない）
    private readonly ulong[,] _edgeHashes;   // ノードi-j間にエッジがある時
    private readonly ulong _wolfTurnHash;    // オオカミのターンの時にXOR

    public ZobristHasher(int maxNodes)
    {
        // シード固定で毎回同じ乱数テーブルを生成
        var rnd = new Random(12345);

        _rabbitHashes = new ulong[maxNodes];
        _wolfHashes = new ulong[maxNodes];
        _edgeHashes = new ulong[maxNodes, maxNodes];
        _wolfTurnHash = NextUInt64(rnd);

        for (int i = 0; i < maxNodes; i++)
        {
            _rabbitHashes[i] = NextUInt64(rnd);
            _wolfHashes[i] = NextUInt64(rnd);

            for (int j = i + 1; j < maxNodes; j++)
                _edgeHashes[i, j] = NextUInt64(rnd);
        }
    }

    // ハッシュ値の取得
    public ulong GetRabbitHash(int nodeId) => _rabbitHashes[nodeId];
    public ulong GetWolfHash(int nodeId) => _wolfHashes[nodeId];
    public ulong GetTurnHash() => _wolfTurnHash;

    public ulong GetEdgeHash(int nodeA, int nodeB)
    {
        // 正規化（EdgeDataと同じ順序保証）
        int a = Math.Min(nodeA, nodeB);
        int b = Math.Max(nodeA, nodeB);
        return _edgeHashes[a, b];
    }

    /// <summary>盤面全体からハッシュを一から計算する（初期化時のみ使用）</summary>
    public ulong ComputeFullHash(GraphModel graph, bool isWolfTurn)
    {
        ulong hash = 0;

        hash ^= GetRabbitHash(graph.GetPlayerPosition(PlayerID.Rabbit));
        hash ^= GetWolfHash(graph.GetPlayerPosition(PlayerID.WolfA));
        hash ^= GetWolfHash(graph.GetPlayerPosition(PlayerID.WolfB));

        foreach (var edge in graph.Edges)
            hash ^= GetEdgeHash(edge.NodeA, edge.NodeB);

        if (isWolfTurn)
            hash ^= GetTurnHash();

        return hash;
    }

    // ulong 乱数生成
    private static ulong NextUInt64(Random rnd)
    {
        Span<byte> buf = stackalloc byte[8];
        rnd.NextBytes(buf);
        return BitConverter.ToUInt64(buf);
    }
}
