using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class HumanInputStrategy : IPlayerInputStrategy
{
    private readonly INodeInputReceiver _input;
    private readonly RuleEngine _rule;
    private readonly ZobristHasher _hasher;

    private ActionMode _mode = ActionMode.Move;

    public HumanInputStrategy(
        INodeInputReceiver input,
        RuleEngine rule,
        ZobristHasher hasher)
    {
        _input = input;
        _rule = rule;
        _hasher = hasher;

        _input.OnModeChangeRequested += SetMode;
    }

    public async UniTask<IGameCommand> DecideActionAsync(
        GraphModel graph,
        PlayerType playerType,
        CancellationToken ct)
    {
        _input.SetActive(true);

        try
        {
            while (true)
            {
                ct.ThrowIfCancellationRequested();

                PlayerID? pickedPieceId;
                int nodeId;
                if (playerType == PlayerType.Rabbit)
                {
                    pickedPieceId = PlayerID.Rabbit;
                }
                else
                {
                    // 狼はどちらの駒を動かすか選べる
                    nodeId = await _input.WaitForNodeClickAsync(ct);
                    pickedPieceId = TryGetPlayerID(graph, nodeId);
                    if (pickedPieceId == null)
                    {
                        // TODO: ユーザーフィードバックを実装
                        Debug.Log("オオカミのいるノードではありません");
                        continue;
                    }

                    // TODO: 選択した方をハイライト表示する
                }

                nodeId = await _input.WaitForNodeClickAsync(ct);

                IGameCommand command = TryBuildCommand(graph, (PlayerID)pickedPieceId, nodeId);
                if (command == null)
                {
                    // TODO: ユーザーフィードバックを実装
                    Debug.Log("無効なクリック");
                    continue;
                }

                return command;
            }
        }
        finally
        {
            _input.SetActive(false);
        }
    }

    /// <summary>
    /// クリックからCommandを組み立てる
    /// </summary>
    /// <param name="nodeId">クリックされたノードのID</param>
    /// <returns></returns>
    private IGameCommand TryBuildCommand(GraphModel graph, PlayerID playerId, int nodeId)
    {
        int playerNode = graph.GetPlayerPosition(playerId);

        switch (_mode)
        {
            case ActionMode.Move:
                if (_rule.CanMove(graph, playerId, nodeId))
                {
                    return new MoveCommand(playerId, playerNode, nodeId, _hasher);
                }
                break;

            case ActionMode.Disconnect:
                if (_rule.CanDisconnect(graph, playerNode, nodeId))
                {
                    return new DisconnectCommand(playerId, new EdgeData(playerNode, nodeId), _hasher);
                }

                break;


            case ActionMode.Connect:
                if (_rule.CanConnect(graph, playerId, nodeId))
                {
                    return new ConnectCommand(playerId, new EdgeData(playerNode, nodeId), _hasher);
                }
                break;
        }
        return null;
    }

    private PlayerID? TryGetPlayerID(GraphModel graph, int nodeId)
    {
        if (nodeId == graph.GetPlayerPosition(PlayerID.Rabbit))
        {
            return PlayerID.Rabbit;
        }
        else if (nodeId == graph.GetPlayerPosition(PlayerID.WolfA))
        {
            return PlayerID.WolfA;
        }
        else if (nodeId == graph.GetPlayerPosition(PlayerID.WolfB))
        {
            return PlayerID.WolfB;
        }
        return null;
    }

    public void SetMode(ActionMode mode) => _mode = mode;
}
