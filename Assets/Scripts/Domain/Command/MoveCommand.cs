public class MoveCommand : IGameCommand
{
    private readonly PlayerType _playerType;
    private readonly int _from;
    private readonly int _to;

    public MoveCommand(PlayerType playerType, int from, int to)
    {
        _playerType = playerType;
        _from = from;
        _to = to;
    }

    public void Execute(GraphModel graph) =>
        graph.MovePlayer(_playerType, _to);

    public void Undo(GraphModel graph) =>
        graph.MovePlayer(_playerType, _from);
}
