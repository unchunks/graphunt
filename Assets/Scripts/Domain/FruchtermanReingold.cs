using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class FruchtermanReingold
{
    private const float EPS = 0.01f;

    public static Dictionary<int, Vector3> Solve(
        IReadOnlyList<int> ids,
        IReadOnlyList<(int a, int b)> edges,
        float radius = 5f,   // 配置空間の半径
        int maxIterations = 500)
    {
        int n = ids.Count;
        if (n == 0) return new Dictionary<int, Vector3>();

        // 球の体積からkを計算
        float volume = (4f / 3f) * Mathf.PI * radius * radius * radius;
        float k = Mathf.Pow(volume / n, 1f / 3f);
        float temperature = radius * 0.5f;

        var pos = InitializeSphere(ids, radius);
        var disp = ids.ToDictionary(id => id, _ => Vector3.zero);

        int iter = 0;
        while (temperature > 0.05f && iter++ < maxIterations)
        {
            Step(ids, edges, pos, disp, k, ref temperature);
        }

        return pos;
    }

    // ─── 初期配置：球面上にランダム配置 ─────────────────
    private static Dictionary<int, Vector3> InitializeSphere(
        IReadOnlyList<int> ids, float radius)
    {
        var dict = new Dictionary<int, Vector3>();
        var rng = new System.Random(42);  // シード固定で毎回同じ初期配置

        for (int i = 0; i < ids.Count; i++)
        {
            // 球面上の一様ランダム点（Marsaglia法）
            float theta = (float)(rng.NextDouble() * 2 * Mathf.PI);
            float phi = Mathf.Acos((float)(2 * rng.NextDouble() - 1));
            float r = radius * 0.8f;

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
                Vector3 force = delta.normalized * (k * k / dist);

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

        // ③ エッジ近接ペナルティ
        // 無関係なノードがエッジに近すぎる場合、エッジから遠ざける力を追加
        ApplyEdgeRepulsion(ids, edges, pos, disp, k);

        // 位置の更新
        foreach (var id in ids)
        {
            Vector3 d = disp[id];
            float len = d.magnitude;
            if (len > EPS)
                pos[id] += d.normalized * Mathf.Min(len, temperature);
        }

        temperature *= 0.95f;
    }

    // ─── エッジ近接ペナルティ ─────────────────────────────
    // 無関係なノードがエッジの近くにいる場合、反発力でエッジから遠ざける
    private static void ApplyEdgeRepulsion(
        IReadOnlyList<int> ids,
        IReadOnlyList<(int a, int b)> edges,
        Dictionary<int, Vector3> pos,
        Dictionary<int, Vector3> disp,
        float k)
    {
        float threshold = k * 1.5f;  // この距離以内なら反発する

        foreach (var (a, b) in edges)
        {
            Vector3 edgeStart = pos[a];
            Vector3 edgeEnd = pos[b];
            Vector3 edgeDir = edgeEnd - edgeStart;
            float edgeLen = Mathf.Max(EPS, edgeDir.magnitude);

            foreach (var id in ids)
            {
                // エッジの端点は無視
                if (id == a || id == b) continue;

                // ノードからエッジへの最近傍点を求める
                Vector3 toNode = pos[id] - edgeStart;
                float t = Mathf.Clamp01(Vector3.Dot(toNode, edgeDir) / (edgeLen * edgeLen));
                Vector3 closest = edgeStart + t * edgeDir;

                Vector3 repulsion = pos[id] - closest;
                float dist = Mathf.Max(EPS, repulsion.magnitude);

                if (dist < threshold)
                {
                    // 距離が近いほど強い反発力
                    float strength = (threshold - dist) / threshold;
                    disp[id] += repulsion.normalized * strength * k * 0.5f;
                }
            }
        }
    }
}
