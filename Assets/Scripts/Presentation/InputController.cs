using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour, INodeInputReceiver
{
    [SerializeField] private Camera _camera;

    public event Action<ActionMode> OnModeChangeRequested;

    private UniTaskCompletionSource<int> _tcs;
    private bool _isActive = false;

    public void SetActive(bool active) => _isActive = active;

    public void RequestModeChange(ActionMode mode) => OnModeChangeRequested?.Invoke(mode);

    public UniTask<int> WaitForNodeClickAsync(CancellationToken ct)
    {
        _tcs = new UniTaskCompletionSource<int>();
        ct.Register(() => _tcs.TrySetCanceled());
        return _tcs.Task;
    }

    private void Update()
    {
        if (!_isActive) return;
        if (_tcs == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.TryGetComponent<NodeView>(out NodeView nodeView))
            {
                _tcs.TrySetResult(nodeView.NodeId);
            }
        }
    }
}