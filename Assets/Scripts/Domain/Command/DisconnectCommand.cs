public class DisconnectCommand : IGameCommand
{
    private readonly PlayerID _playerId;
    private readonly int _from;
    private readonly int _to;

    public DisconnectCommand(PlayerID playerId, int from, int to)
    {
        _playerId = playerId;
        _from = from;
        _to = to;
    }

    public void Execute(GraphModel graph) =>
        graph.RemoveEdge(_from, _to);

    public void Undo(GraphModel graph) =>
        graph.AddEdge(_from, _to);
}
