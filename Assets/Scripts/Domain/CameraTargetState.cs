using UnityEngine;

/// <summary>
/// カメラが「最終的にどうなりたいか」を保持するクラス
/// 入力や補間とは無関係な、純粋な目標値のみを管理する
/// </summary>
public class CameraTargetState
{
    // 目標となるワールド座標
    public Vector3 Position;

    // 目標ズーム値（今回はY座標＝高さ）
    public float Zoom;

    // Y軸回転量（Yaw）
    public float Yaw;

    // 目標回転
    public Quaternion Rotation;

    /// <summary>
    /// 現在のTransformの状態を初期値として取り込む
    /// </summary>
    public void Initialize(Transform t)
    {
        Position = t.position;
        Zoom = t.position.y;
        Yaw = t.eulerAngles.y;
        Rotation = t.rotation;
    }

    /// <summary>
    /// Yawをもとに、現在のX/Z角度を維持した回転を生成
    /// </summary>
    public void UpdateRotation(float xAngle, float zAngle)
    {
        Rotation = Quaternion.Euler(xAngle, Yaw, zAngle);
    }
}
