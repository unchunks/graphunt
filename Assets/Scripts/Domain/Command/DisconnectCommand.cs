public class DisconnectCommand : IGameCommand
{
    private readonly PlayerID _playerId;
    private readonly EdgeData _edge;
    private readonly ZobristHasher _hasher;

    public DisconnectCommand(PlayerID playerId, EdgeData edge, ZobristHasher hasher)
    {
        _playerId = playerId;
        _edge = edge;
        _hasher = hasher;
    }

    public void Execute(GraphModel graph)
    {
        graph.CurrentHash ^= _hasher.GetEdgeHash(_edge.NodeA, _edge.NodeB);
        graph.RemoveEdge(_edge);
        graph.CurrentHash ^= _hasher.GetTurnHash();
    }

    public void Undo(GraphModel graph)
    {
        graph.CurrentHash ^= _hasher.GetEdgeHash(_edge.NodeA, _edge.NodeB);
        graph.AddEdge(_edge);
        graph.CurrentHash ^= _hasher.GetTurnHash();
    }
}
