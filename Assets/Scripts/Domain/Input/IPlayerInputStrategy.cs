using Cysharp.Threading.Tasks;
using System.Threading;

public interface IPlayerInputStrategy
{
    UniTask<IGameCommand> DecideActionAsync(
        GraphModel graph,
        PlayerID playerId,
        CancellationToken cancellationToken);
}
