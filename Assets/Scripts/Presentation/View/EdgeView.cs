using System;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EdgeView : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    public void Initialize(EdgeData edge, NodeView nodeA, NodeView nodeB)
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
        _lineRenderer.startWidth = 0.05f;
        _lineRenderer.endWidth = 0.05f;

        UpdatePositions(nodeA, nodeB);
    }

    private void UpdatePositions(NodeView nodeA, NodeView nodeB)
    {
        // TODO: ほかのエッジやノードと重ならないよう、曲線にする
        _lineRenderer.SetPosition(0, nodeA.transform.position);
        _lineRenderer.SetPosition(1, nodeB.transform.position);
    }

    public void UpdatePositions(Vector3 a, Vector3 b)
    {
        _lineRenderer.SetPosition(0, a);
        _lineRenderer.SetPosition(1, b);
    }
}
