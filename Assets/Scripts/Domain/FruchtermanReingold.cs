using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class FruchtermanReingold
{
    private const float EPS = 0.01f;

    public static Dictionary<int, Vector3> Solve(
        IReadOnlyList<int> ids,
        IReadOnlyList<(int a, int b)> edges,
        float radius = 8f,
        int maxIterations = 300)
    {
        int n = ids.Count;
        if (n == 0) return new Dictionary<int, Vector3>();

        // 隣接セットを事前構築（エッジ近接ペナルティの高速化）
        var adjacency = BuildAdjacency(ids, edges);

        float volume = (4f / 3f) * Mathf.PI * radius * radius * radius;
        float k = Mathf.Pow(volume / n, 1f / 3f);
        float temperature = radius * 0.5f;

        var pos = InitializeSphere(ids, radius);
        var disp = ids.ToDictionary(id => id, _ => Vector3.zero);

        int iter = 0;
        while (temperature > 0.05f && iter++ < maxIterations)
        {
            Step(ids, edges, adjacency, pos, disp, k, ref temperature);
        }

        return pos;
    }

    // ─── 隣接セットの事前構築 ────────────────────────────
    // エッジ近接ペナルティで「端点かどうか」を O(1) で判定するために使う
    private static HashSet<(int, int)> BuildAdjacency(
        IReadOnlyList<int> ids,
        IReadOnlyList<(int a, int b)> edges)
    {
        var set = new HashSet<(int, int)>();
        foreach (var (a, b) in edges)
        {
            set.Add((a, b));
            set.Add((b, a));
        }
        return set;
    }

    // ─── 初期配置：球面上に黄金角螺旋で均等配置 ─────────
    // ランダム配置より初期状態が均等なため、収束が早い
    private static Dictionary<int, Vector3> InitializeSphere(
        IReadOnlyList<int> ids,
        float radius)
    {
        var dict = new Dictionary<int, Vector3>();
        int n = ids.Count;
        float goldenRatio = (1f + Mathf.Sqrt(5f)) / 2f;

        for (int i = 0; i < n; i++)
        {
            // フィボナッチ格子法による球面均等分布
            float theta = 2f * Mathf.PI * i / goldenRatio;
            float phi = Mathf.Acos(1f - 2f * (i + 0.5f) / n);
            float r = radius * 0.85f;

            dict[ids[i]] = new Vector3(
                r * Mathf.Sin(phi) * Mathf.Cos(theta),
                r * Mathf.Sin(phi) * Mathf.Sin(theta),
                r * Mathf.Cos(phi)
            );
        }

        return dict;
    }

    // ─── 1ステップの力計算 ───────────────────────────────
    private static void Step(
        IReadOnlyList<int> ids,
        IReadOnlyList<(int a, int b)> edges,
        HashSet<(int, int)> adjacency,
        Dictionary<int, Vector3> pos,
        Dictionary<int, Vector3> disp,
        float k,
        ref float temperature)
    {
        foreach (var id in ids) disp[id] = Vector3.zero;

        // 斥力（全ノード間）
        for (int i = 0; i < ids.Count; i++)
            for (int j = i + 1; j < ids.Count; j++)
            {
                int v = ids[i];
                int u = ids[j];

                Vector3 delta = pos[v] - pos[u];
                float dist = Mathf.Max(EPS, delta.magnitude);

                // 近すぎる場合はランダム方向に強く弾く（重なり防止）
                Vector3 force = dist < EPS * 10f
                    ? Random.insideUnitSphere * k
                    : delta.normalized * (k * k / dist);

                disp[v] += force;
                disp[u] -= force;
            }

        // 引力（エッジで繋がったノード間）
        foreach (var (a, b) in edges)
        {
            Vector3 delta = pos[a] - pos[b];
            float dist = Mathf.Max(EPS, delta.magnitude);
            Vector3 force = delta.normalized * (dist * dist / k);

            disp[a] -= force;
            disp[b] += force;
        }

        // エッジ近接ペナルティ
        ApplyEdgeRepulsion(ids, edges, adjacency, pos, disp, k);

        // 位置の更新
        foreach (var id in ids)
        {
            Vector3 d = disp[id];
            float len = d.magnitude;
            if (len > EPS)
                pos[id] += d.normalized * Mathf.Min(len, temperature);
        }

        temperature *= 0.92f;  // 冷却をやや遅くして収束品質を上げる
    }

    // ─── エッジ近接ペナルティ ─────────────────────────────
    private static void ApplyEdgeRepulsion(
        IReadOnlyList<int> ids,
        IReadOnlyList<(int a, int b)> edges,
        HashSet<(int, int)> adjacency,
        Dictionary<int, Vector3> pos,
        Dictionary<int, Vector3> disp,
        float k)
    {
        // ノード数が多いほどペナルティ範囲を広げる
        float threshold = k * 2.0f;

        foreach (var (a, b) in edges)
        {
            Vector3 edgeStart = pos[a];
            Vector3 edgeVec = pos[b] - edgeStart;
            float edgeLenSq = Mathf.Max(EPS, edgeVec.sqrMagnitude);

            foreach (var id in ids)
            {
                // 端点および隣接ノードは無視
                if (id == a || id == b) continue;
                if (adjacency.Contains((id, a)) || adjacency.Contains((id, b))) continue;

                // エッジへの最近傍点
                float t = Mathf.Clamp01(Vector3.Dot(pos[id] - edgeStart, edgeVec) / edgeLenSq);
                Vector3 closest = edgeStart + t * edgeVec;
                Vector3 repulsion = pos[id] - closest;
                float dist = Mathf.Max(EPS, repulsion.magnitude);

                if (dist < threshold)
                {
                    // 距離の2乗に反比例する反発力（近いほど急激に強くなる）
                    float strength = (threshold * threshold) / (dist * dist) - 1f;
                    disp[id] += repulsion.normalized * strength * k * 0.3f;
                }
            }
        }
    }
}
