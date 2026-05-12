using UnityEngine;

/// <summary>
/// 各クラスを統括し、処理の流れを制御するクラス。
/// 「何をするか」はここに、「どうやるか」は他クラスに分離されている。
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float _panSpeed = 100f;
    [SerializeField] float _rotSpeed = 100f;
    [SerializeField] float _zoomSpeed = 100f;

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
        _motor.Apply(transform, _state);
    }

    /// <summary>
    /// 入力値をもとに、TargetStateを更新する
    /// </summary>
    void UpdateTargetState()
    {
        // マウス中ボタンドラッグによる平行移動（カメラの向いている面に対して平行）
        if (_input.DragPanInput != Vector2.zero)
        {
            // カメラ自身の右方向(right)と上方向(up)を基準に移動ベクトルを作る
            Vector3 dragDir = (transform.right * _input.DragPanInput.x) + (transform.up * _input.DragPanInput.y);
            Vector3 moveDelta = dragDir * _panSpeed * Time.deltaTime;

            // X, Z 軸は Position へ反映
            _state.Position += moveDelta;

            // Y 軸（高さ）は Zoom として補間・管理されているため、Zoom に加算する
            _state.Zoom += moveDelta.y;

            ApplyBounds();
        }

        // スクロールによるズーム
        if (_input.ZoomInput != 0)
        {
            // transform.forward (現在のカメラの向き) を使って移動ベクトルを計算
            // ZoomInput が正（奥に回す）のとき前進、負（手前に回す）のとき後退
            Vector3 zoomDelta = transform.forward * _input.ZoomInput * _zoomSpeed * Time.deltaTime;

            // XZ平面の座標を Position に反映
            _state.Position += zoomDelta;

            // Y軸（高さ）は独立して管理されている Zoom 変数に反映
            _state.Zoom += zoomDelta.y;
        }

        // 回転処理（右ドラッグ）
        if (_input.RotateInput != Vector2.zero)
        {
            _state.Yaw += _input.RotateInput.x * _rotSpeed * Time.deltaTime;
            _state.Pitch -= _input.RotateInput.y * _rotSpeed * Time.deltaTime;

            // カメラがひっくり返らないように制限
            _state.Pitch = Mathf.Clamp(_state.Pitch, -85f, 85f);

            _state.UpdateRotation();
        }
    }

    /// <summary>
    /// 境界（Bounds）の制限を適用する
    /// </summary>
    private void ApplyBounds()
    {
        if (_useBounds)
        {
            _state.Position.x = Mathf.Clamp(_state.Position.x, _bounds.min.x, _bounds.max.x);
            _state.Position.z = Mathf.Clamp(_state.Position.z, _bounds.min.z, _bounds.max.z);
            // ※必要であればここで _state.Zoom（高さ）に対しても Clamp をかけることができます
        }
    }
}
