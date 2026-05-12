using UnityEngine;

/// <summary>
/// カメラが「最終的にどうなりたいか」を保持するクラス。
/// 入力や補間とは無関係な、純粋な目標値のみを管理する。
/// </summary>
public class CameraTargetState
{
    // 目標となるワールド座標
    public Vector3 Position;

    // 目標ズーム値（今回はY座標＝高さ）
    public float Zoom;

    // Y軸回転量（Yaw）
    public float Yaw;

    // X軸回転量（Pitch）
    public float Pitch;

    // 目標回転
    public Quaternion Rotation;

    /// <summary>
    /// 現在のTransformの状態を初期値として取り込む
    /// </summary>
    public void Initialize(Transform t)
    {
        Position = t.position;
        Zoom = t.position.y;

        // 現在の角度を取得し、Pitchを -180 ~ 180 の範囲で初期化
        Vector3 euler = t.eulerAngles;
        Pitch = euler.x > 180 ? euler.x - 360 : euler.x;
        Yaw = euler.y;

        Rotation = t.rotation;
    }

    /// <summary>
    /// 現在のPitchとYawからRotationを更新
    /// </summary>
    public void UpdateRotation()
    {
        // Z軸は0で固定
        Rotation = Quaternion.Euler(Pitch, Yaw, 0f);
    }
}
