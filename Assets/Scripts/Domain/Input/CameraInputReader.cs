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

    // ズーム入力（マウスホイール）
    public float ZoomInput;

    // 回転入力
    public float RotateInput;

    /// <summary>
    /// 現在フレームの入力を取得する
    /// </summary>
    public void Read(bool useEdge, float edgeWidth)
    {
        PanInput = Vector2.zero;
        ZoomInput = 0f;
        RotateInput = 0f;

        var kb = Keyboard.current;
        var mouse = Mouse.current;

        // キーボードパン
        if (kb != null)
        {
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed) PanInput.y += 1;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed) PanInput.y -= 1;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) PanInput.x += 1;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) PanInput.x -= 1;

            if (kb.qKey.isPressed) RotateInput += 1;
            if (kb.eKey.isPressed) RotateInput -= 1;
        }

        // マウスホイールズーム
        if (mouse != null)
        {
            ZoomInput = mouse.scroll.ReadValue().y;

            // マウスドラッグ回転
            if (mouse.middleButton.isPressed)
            {
                RotateInput = mouse.delta.ReadValue().x * 0.1f;
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
