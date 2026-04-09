using UnityEngine;

public class GraphMaterialRegistry: MonoBehaviour
{
    [SerializeField] private Material _nodeDefault;
    [SerializeField] private Material _nodeHighlight;
    [SerializeField] private Material _nodeDimmed;
    [SerializeField] private Material _edgeDefault;
    [SerializeField] private Material _edgeFlow;

    public Material GetNodeMaterial(NodeVisualState state) => state switch
    {
        NodeVisualState.Default => _nodeDefault,
        NodeVisualState.Highlight => _nodeHighlight,
        NodeVisualState.Dimmed => _nodeDimmed,
        _ => throw new System.ArgumentOutOfRangeException(nameof(state), state, null)
    };

    public Material GetEdgeMaterial(EdgeVisualState state) => state switch
    {
        EdgeVisualState.Default => _edgeDefault,
        EdgeVisualState.Flow => _edgeFlow,
        _ => throw new System.ArgumentOutOfRangeException(nameof(state), state, null)
    };
}
