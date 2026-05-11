using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private InputController _inputController;

    [SerializeField] private Button _moveButton;
    [SerializeField] private Button _disconnectButton;
    [SerializeField] private Button _connectButton;

    private void Start()
    {
        _moveButton.onClick.AddListener(() => _inputController.RequestModeChange(ActionMode.Move));
        _disconnectButton.onClick.AddListener(() => _inputController.RequestModeChange(ActionMode.Disconnect));
        _connectButton.onClick.AddListener(() => _inputController.RequestModeChange(ActionMode.Connect));
    }

}
