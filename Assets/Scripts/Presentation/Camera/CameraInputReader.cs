using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 入力を読むこと「だけ」を責務に持つクラス。
/// カメラの状態やTransformには一切触れない。
/// </summary>
public class CameraInputReader
{
    // パン入力（x:左右, y:前後）
    public Vector2 PanInput;

    // マウスドラッグによるパン入力
    public Vector2 DragPanInput;

    // ズーム入力（マウスホイール）
    public float ZoomInput;

    // 回転入力
    public Vector2 RotateInput;

    /// <summary>
    /// 現在フレームの入力を取得する
    /// </summary>
    public void Read(bool useEdge, float edgeWidth)
    {
        PanInput = Vector2.zero;
        DragPanInput = Vector2.zero;
        ZoomInput = 0f;
        RotateInput = Vector2.zero;

        var mouse = Mouse.current;

        // マウス入力
        if (mouse != null)
        {
            ZoomInput = mouse.scroll.ReadValue().y;

            // 右ドラッグで上下左右の回転を取得
            if (mouse.rightButton.isPressed)
            {
                // x: 左右(Yaw), y: 上下(Pitch)
                RotateInput = mouse.delta.ReadValue() * 0.1f;
            }

            if (mouse.middleButton.isPressed)
            {
                DragPanInput = new Vector2(-mouse.delta.ReadValue().x, -mouse.delta.ReadValue().y);
            }
        }

        // 画面端パン
        if (useEdge && mouse != null)
        {
            Vector2 m = mouse.position.ReadValue();

            if (m.y >= Screen.height - edgeWidth) PanInput.y += 1;
            if (m.y <= edgeWidth) PanInput.y -= 1;
            if (m.x >= Screen.width - edgeWidth) PanInput.x += 1;
            if (m.x <= edgeWidth) PanInput.x -= 1;
        }

        PanInput = PanInput.normalized;
    }
}
