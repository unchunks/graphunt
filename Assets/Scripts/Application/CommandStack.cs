using System.Collections.Generic;

public class CommandStack
{
    private readonly Stack<IGameCommand> _undoStack = new();
    private readonly Stack<IGameCommand> _redoStack = new();
    private const int MaxStackSize = 100;

    public bool CanUndo => _undoStack.Count > 0;
    public bool CanRedo => _redoStack.Count > 0;

    public void Execute(IGameCommand command, GraphModel graph)
    {
        command.Execute(graph);
        _undoStack.Push(command);
        _redoStack.Clear();  // 新しいコマンドを実行したら、Redoスタックはクリア

        if (_undoStack.Count > MaxStackSize)
        {
            // 逆順でスタックがコピーされるので、最も古いコマンドが最後に来る
            var temp = new Stack<IGameCommand>(_undoStack);
            temp.Pop();
            _undoStack.Clear();
            foreach (var cmd in temp)
                _undoStack.Push(cmd);
        }
    }

    public void Undo(GraphModel graph)
    {
        if (_undoStack.Count == 0) return;

        var command = _undoStack.Pop();
        command.Undo(graph);
        _redoStack.Push(command);
    }

    public void Redo(GraphModel graph)
    {
        if (_redoStack.Count == 0) return;

        var command = _redoStack.Pop();
        command.Execute(graph);
        _undoStack.Push(command);
    }
}