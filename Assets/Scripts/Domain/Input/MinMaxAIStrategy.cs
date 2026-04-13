using Cysharp.Threading.Tasks;
using System.Threading;

public class MinMaxAIStrategy : IPlayerInputStrategy
{
    UniTask<IGameCommand> IPlayerInputStrategy.DecideActionAsync(
        GraphModel graph,
        PlayerID playerId,
        CancellationToken cancellationToken)
    {
        return new UniTask<IGameCommand>(); //DecideActionAsync(graph, playerId, cancellationToken);
    }
}
