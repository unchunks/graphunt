// Domain/Command/ConnectCommand.cs
public class ConnectCommand : IGameCommand
{
    private readonly PlayerID _playerId;
    private readonly EdgeData _edge;
    private readonly ZobristHasher _hasher;

    public ConnectCommand(PlayerID playerId, EdgeData edge, ZobristHasher hasher)
    {
        _playerId = playerId;
        _edge = edge;
        _hasher = hasher;
    }

    public void Execute(GraphModel graph)
    {
        graph.AddEdge(_edge);
        graph.CurrentHash ^= _hasher.GetEdgeHash(_edge.NodeA, _edge.NodeB);
        graph.CurrentHash ^= _hasher.GetTurnHash();
    }

    public void Undo(GraphModel graph)
    {
        graph.RemoveEdge(_edge);
        graph.CurrentHash ^= _hasher.GetEdgeHash(_edge.NodeA, _edge.NodeB);
        graph.CurrentHash ^= _hasher.GetTurnHash();
    }
}
