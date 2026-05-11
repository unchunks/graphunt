public class DisconnectCommand : IGameCommand
{
    private readonly PlayerType _playerType;
    private readonly EdgeData _edgeData;

    public DisconnectCommand(PlayerType playerType, EdgeData edgeData)
    {
        _playerType = playerType;
        _edgeData = edgeData;
    }

    public void Execute(GraphModel graph) =>
        graph.RemoveEdge(_edgeData);

    public void Undo(GraphModel graph) =>
        graph.AddEdge(_edgeData);
}
