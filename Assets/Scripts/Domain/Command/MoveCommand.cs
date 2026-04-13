using Unity.VisualScripting;

public class MoveCommand : IGameCommand
{
    private readonly PlayerID _playerId;
    private readonly int _from;
    private readonly int _to;

    public MoveCommand(PlayerID playerId, int from, int to)
    {
        _playerId = playerId;
        _from = from;
        _to = to;
    }

    public void Execute(GraphModel graph) =>
        graph.MovePlayer(_playerId, _to);

    public void Undo(GraphModel graph) =>
        graph.MovePlayer(_playerId, _from);
}
