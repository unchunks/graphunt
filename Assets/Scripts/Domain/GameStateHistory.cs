using System.Collections.Generic;

public class GameStateHistory
{
    // ハッシュごとの出現回数
    private readonly Dictionary<ulong, int> _counts = new();

    // Undo 用に順序を保持
    private readonly Stack<ulong> _history = new();

    /// <summary>
    /// 現在のハッシュを履歴に追加する。
    /// 同一局面が3回目なら千日手（true を返す）。
    /// </summary>
    public bool TryRegister(ulong hash)
    {
        // 履歴に積む（Undoのため必須）
        _history.Push(hash);

        if (_counts.TryGetValue(hash, out int count))
        {
            count++;
            _counts[hash] = count;

            // 3回目で千日手
            return count >= 3;
        }
        else
        {
            _counts[hash] = 1;
            return false;
        }
    }

    /// <summary>
    /// Undo時に直前の盤面状態を履歴から取り除く
    /// </summary>
    public void PopLast()
    {
        if (_history.Count == 0) return;

        ulong last = _history.Pop();

        if (_counts.TryGetValue(last, out int count))
        {
            count--;

            if (count <= 0)
                _counts.Remove(last);
            else
                _counts[last] = count;
        }
    }

    /// <summary>
    /// 履歴を完全にクリア
    /// </summary>
    public void Clear()
    {
        _counts.Clear();
        _history.Clear();
    }
}
