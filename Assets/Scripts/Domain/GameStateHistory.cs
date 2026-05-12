using System.Collections.Generic;

public class GameStateHistory
{
    private readonly HashSet<ulong> _hashes = new();
    private readonly Stack<ulong> _history = new();  // Undo用に順序を保持

    /// <summary>
    /// 現在のハッシュを履歴に追加する。
    /// 既に存在していれば千日手（true を返す）。
    /// </summary>
    public bool TryRegister(ulong hash)
    {
        // TODO: 3回目の同一局面で千日手とする
        // Add が false を返す = 既に存在する = 千日手
        return false; // !_hashes.Add(hash);
    }

    /// <summary>Undo時に直前の盤面状態を履歴から取り除く</summary>
    public void PopLast()
    {
        if (_history.Count == 0) return;
        ulong last = _history.Pop();
        _hashes.Remove(last);
    }

    public void Clear()
    {
        _hashes.Clear();
        _history.Clear();
    }
}
