public class MoveCommand : IGameCommand
{
    private readonly PlayerID _playerId;
    private readonly int _from;
    private readonly int _to;
    private readonly ZobristHasher _hasher;

    public MoveCommand(PlayerID playerId, int from, int to, ZobristHasher hasher)
    {
        _playerId = playerId;
        _from = from;
        _to = to;
        _hasher = hasher;
    }

    public void Execute(GraphModel graph)
    {
        graph.CurrentHash ^= GetPlayerHash(_from);
        graph.MovePlayer(_playerId, _to);
        graph.CurrentHash ^= GetPlayerHash(_to);

        // ターン交代
        graph.CurrentHash ^= _hasher.GetTurnHash();
    }

    public void Undo(GraphModel graph)
    {
        graph.CurrentHash ^= GetPlayerHash(_to);
        graph.MovePlayer(_playerId, _from);
        graph.CurrentHash ^= GetPlayerHash(_from);

        graph.CurrentHash ^= _hasher.GetTurnHash();
    }

    private ulong GetPlayerHash(int nodeId) => _playerId == PlayerID.Rabbit
        ? _hasher.GetRabbitHash(nodeId)
        : _hasher.GetWolfHash(nodeId);
}
