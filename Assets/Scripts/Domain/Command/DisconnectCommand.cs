public class DisconnectCommand : IGameCommand
{
    private readonly PlayerID _playerId;
    private readonly EdgeData _edgeData;

    public DisconnectCommand(PlayerID playerId, EdgeData edgeData)
    {
        _playerId = playerId;
        _edgeData = edgeData;
    }

    public void Execute(GraphModel graph) =>
        graph.RemoveEdge(_edgeData);

    public void Undo(GraphModel graph) =>
        graph.AddEdge(_edgeData);
}
