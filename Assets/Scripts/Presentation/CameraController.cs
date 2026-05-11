using UnityEngine;

/// <summary>
/// 各クラスを統括し、処理の流れを制御するクラス
/// 「何をするか」はここに、「どうやるか」は他クラスに分離されている
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float _panSpeed = 20f;
    [SerializeField] float _panSmooth = 0.15f;
    [SerializeField] float _zoomSpeed = 500f;
    [SerializeField] float _zoomSmooth = 0.1f;
    [SerializeField] float _rotSpeed = 100f;
    [SerializeField] float _rotSmooth = 0.1f;

    [Header("Edge Pan")]
    [SerializeField] bool _useEdge = false;
    [SerializeField] float _edgeWidth = 10f;

    [Header("Bounds")]
    [SerializeField] bool _useBounds = false;
    [SerializeField] Bounds _bounds;

    private CameraTargetState _state = new();
    private CameraInputReader _input = new();
    private CameraMotor _motor = new();

    void Awake()
    {
        // 現在のTransformを初期目標として設定
        _state.Initialize(transform);
    }

    void Update()
    {
        // 入力取得
        _input.Read(_useEdge, _edgeWidth);

        // 入力をもとに目標状態を更新
        UpdateTargetState();
    }

    void LateUpdate()
    {
        // 補間してTransformへ反映
        _motor.Apply(transform, _state, _panSmooth, _zoomSmooth, _rotSmooth);
    }

    /// <summary>
    /// 入力値をもとに、TargetStateを更新する
    /// </summary>
    void UpdateTargetState()
    {
        // パン処理
        if (_input.PanInput != Vector2.zero)
        {
            Vector3 dir = Quaternion.Euler(0, _state.Yaw, 0) *
                          new Vector3(_input.PanInput.x, 0, _input.PanInput.y).normalized;

            _state.Position += dir * _panSpeed * Time.deltaTime;

            if (_useBounds)
            {
                _state.Position.x = Mathf.Clamp(_state.Position.x, _bounds.min.x, _bounds.max.x);
                _state.Position.z = Mathf.Clamp(_state.Position.z, _bounds.min.z, _bounds.max.z);
            }
        }

        // ズーム処理
        if (_input.ZoomInput != 0)
        {
            _state.Zoom -= _input.ZoomInput * _zoomSpeed * Time.deltaTime;
        }

        // 回転処理
        if (_input.RotateInput != 0)
        {
            _state.Yaw += _input.RotateInput * _rotSpeed * Time.deltaTime;
            _state.UpdateRotation(transform.eulerAngles.x, transform.eulerAngles.z);
        }
    }
}
