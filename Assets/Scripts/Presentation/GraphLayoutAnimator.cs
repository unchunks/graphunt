using System.Collections.Generic;
using UnityEngine;

public class GraphLayoutAnimator : MonoBehaviour
{
    [SerializeField] private float _moveDuration = 1.2f;

    private readonly Dictionary<int, Transform> _nodes = new();
    private readonly Dictionary<int, Vector3> _start = new();
    private readonly Dictionary<int, Vector3> _target = new();
    private readonly Dictionary<int, float> _t = new();

    private System.Action _onUpdatedEdges;

    private enum State { Idle, Animating }
    private State _state = State.Idle;

    public void Initialize(
        Dictionary<int, NodeView> nodeViews,
        System.Action onUpdatedEdges)
    {
        _nodes.Clear();
        foreach (var kv in nodeViews)
            _nodes[kv.Key] = kv.Value.transform;

        _onUpdatedEdges = onUpdatedEdges;
    }

    public void Play(Dictionary<int, Vector3> targets)
    {
        foreach (var id in _nodes.Keys)
        {
            _start[id] = _nodes[id].position;
            _target[id] = targets[id];
            _t[id] = 0f;
        }

        _state = State.Animating;
    }

    void Update()
    {
        if (_state != State.Animating) return;

        bool moving = false;

        foreach (var id in _nodes.Keys)
        {
            _t[id] += Time.deltaTime / _moveDuration;
            float t = Mathf.Clamp01(_t[id]);
            float eased = EaseInOut(t);

            _nodes[id].position =
                Vector3.LerpUnclamped(_start[id], _target[id], eased);

            if (t < 1f) moving = true;
        }

        _onUpdatedEdges?.Invoke();

        if (!moving)
            _state = State.Idle;
    }

    //private static float EaseInOut(float t)
    //    => t * t * (3f - 2f * t);

    private static float EaseInOut(float t)
    {
        return t < 0.5f
            ? 4f * t * t * t
            : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
    }
}
