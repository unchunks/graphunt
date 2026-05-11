public class ConnectCommand : IGameCommand
{
    private readonly PlayerType _playerType;
    private readonly EdgeData _edgeData;

    public ConnectCommand(PlayerType playerType, EdgeData edgeData)
    {
        _playerType = playerType;
        _edgeData = edgeData;
    }

    public void Execute(GraphModel graph) =>
        graph.AddEdge(_edgeData);

    public void Undo(GraphModel graph) =>
        graph.RemoveEdge(_edgeData);
}
