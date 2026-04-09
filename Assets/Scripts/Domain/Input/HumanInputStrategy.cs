using Cysharp.Threading.Tasks;
using System.Threading;

public class HumanInputStrategy: IPlayerInputStrategy
{
    UniTask<IGameCommand> IPlayerInputStrategy.DecideActionAsync(
        GraphModel graph,
        PlayerID playerId,
        CancellationToken cancellationToken)
    {
        return DecideActionAsync(graph, playerId, cancellationToken);
    }
}
