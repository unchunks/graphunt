using UnityEngine;

/// <summary>
/// TargetStateに向かってTransformを補間する責務を持つ
/// 実際の「動きの質」はこのクラスで決まる
/// </summary>
public class CameraMotor
{
    private Vector3 _posVelocity;
    private float _zoomVelocity;

    /// <summary>
    /// Transformへ補間結果を適用
    /// </summary>
    public void Apply(
        Transform t,
        CameraTargetState state,
        float panSmooth,
        float zoomSmooth,
        float rotSmooth)
    {
        Vector3 current = t.position;

        // XZ平面の移動補間
        Vector3 pos = Vector3.SmoothDamp(
            new Vector3(current.x, 0, current.z),
            new Vector3(state.Position.x, 0, state.Position.z),
            ref _posVelocity,
            panSmooth
        );

        // ズーム（高さ）の補間
        float zoom = Mathf.SmoothDamp(
            current.y,
            state.Zoom,
            ref _zoomVelocity,
            zoomSmooth
        );

        t.position = new Vector3(pos.x, zoom, pos.z);

        // EaseInOutによる自然な回転
        float ease = EaseInOut(Time.deltaTime / rotSmooth);
        t.rotation = Quaternion.Slerp(t.rotation, state.Rotation, ease);
    }

    /// <summary>
    /// 最初と最後がゆっくりになる補間関数（SmoothStep）
    /// </summary>
    private float EaseInOut(float t)
    {
        return t * t * (3f - 2f * t);
    }
}
