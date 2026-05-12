using UnityEngine;

/// <summary>
/// TargetStateの値をTransformに適用する責務を持つ。
/// パンと回転は補間せず、即座に追従する。
/// </summary>
public class CameraMotor
{
    private float _zoomVelocity;

    /// <summary>
    /// Transformへ結果を適用
    /// </summary>
    public void Apply(
        Transform t,
        CameraTargetState state)
    {
        // 位置を適用
        t.position = new Vector3(state.Position.x, state.Zoom, state.Position.z);

        // 回転を適用
        t.rotation = state.Rotation;
    }
}
