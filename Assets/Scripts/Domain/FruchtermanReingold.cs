using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class FruchtermanReingold
{
    private const float EPS = 0.01f;

    public static Dictionary<int, Vector2> Solve(
        IReadOnlyList<int> ids,
        IReadOnlyList<(int a, int b)> edges,
        Rect area,
        int maxIterations = 500)
    {
        int n = ids.Count;
        float A = area.width * area.height;
        float k = Mathf.Sqrt(A / n);
        float temperature = area.width * 0.15f;

        var pos = Initialize(ids, area);
        var disp = ids.ToDictionary(id => id, _ => Vector2.zero);

        int iter = 0;
        while (temperature > 0.1f && iter++ < maxIterations)
        {
            Step(ids, edges, pos, disp, k, ref temperature);
        }

        return pos;
    }

    private static Dictionary<int, Vector2> Initialize(IReadOnlyList<int> ids, Rect area)
    {
        float radius = Mathf.Min(area.width, area.height) * 0.4f;
        var dict = new Dictionary<int, Vector2>();

        for (int i = 0; i < ids.Count; i++)
        {
            float angle = 2 * Mathf.PI * i / ids.Count;
            dict[ids[i]] = new Vector2(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius
            );
        }

        return dict;
    }

    private static void Step(
        IReadOnlyList<int> ids,
        IReadOnlyList<(int a, int b)> edges,
        Dictionary<int, Vector2> pos,
        Dictionary<int, Vector2> disp,
        float k,
        ref float temperature)
    {
        foreach (var id in ids) disp[id] = Vector2.zero;

        // 斥力
        for (int i = 0; i < ids.Count; i++)
            for (int j = i + 1; j < ids.Count; j++)
            {
                int v = ids[i];
                int u = ids[j];

                Vector2 delta = pos[v] - pos[u];
                float dist = Mathf.Max(EPS, delta.magnitude);
                Vector2 force = delta.normalized * (k * k / dist);

                disp[v] += force;
                disp[u] -= force;
            }

        // 引力
        foreach (var (v, u) in edges)
        {
            Vector2 delta = pos[v] - pos[u];
            float dist = Mathf.Max(EPS, delta.magnitude);
            Vector2 force = delta.normalized * (dist * dist / k);

            disp[v] -= force;
            disp[u] += force;
        }

        // 更新
        foreach (var id in ids)
        {
            Vector2 d = disp[id];
            float len = d.magnitude;
            if (len > EPS)
                pos[id] += d.normalized * Mathf.Min(len, temperature);
        }

        temperature *= 0.95f;
    }
}
