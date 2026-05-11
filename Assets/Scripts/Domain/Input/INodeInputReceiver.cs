using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public interface INodeInputReceiver
{
    UniTask<int> WaitForNodeClickAsync(CancellationToken ct);
    void SetActive(bool active);

    // モード変更通知（UIボタン → HumanInputStrategy）
    event Action<ActionMode> OnModeChangeRequested;
}
