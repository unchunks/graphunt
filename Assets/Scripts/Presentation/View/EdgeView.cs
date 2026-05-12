using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EdgeView : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    [Header("Tube Settings")]
    [SerializeField] private float _tubeWidth = 0.1f;

    public void Initialize(EdgeData edge, NodeView nodeA, NodeView nodeB)
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
        _lineRenderer.startWidth = _tubeWidth;
        _lineRenderer.endWidth = _tubeWidth;
        _lineRenderer.generateLightingData = true;

        UpdatePositions(nodeA, nodeB);
    }

    public void UpdatePositions(NodeView nodeA, NodeView nodeB)
    {
        _lineRenderer.SetPosition(0, nodeA.transform.position);
        _lineRenderer.SetPosition(1, nodeB.transform.position);
    }

    public void UpdatePositions(Vector3 a, Vector3 b)
    {
        _lineRenderer.SetPosition(0, a);
        _lineRenderer.SetPosition(1, b);
    }
}
