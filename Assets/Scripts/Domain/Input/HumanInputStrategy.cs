using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class HumanInputStrategy : IPlayerInputStrategy
{
    private readonly INodeInputReceiver _input;
    private readonly RuleEngine _rule;

    private ActionMode _mode = ActionMode.Move;

    public HumanInputStrategy(INodeInputReceiver input, RuleEngine rule)
    {
        _input = input;
        _rule = rule;

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

                int nodeId = await _input.WaitForNodeClickAsync(ct);

                IGameCommand command = TryBuildCommand(graph, playerType, nodeId);
                if (command != null) return command;

                Debug.Log("無効なクリック");
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
    private IGameCommand TryBuildCommand(GraphModel graph, PlayerType playerType, int nodeId)
    {
        int playerNode = graph.GetPlayerPosition(playerType);

        switch (_mode)
        {
            case ActionMode.Move:
                if (_rule.CanMove(graph, playerType, nodeId))
                {
                    return new MoveCommand(playerType, playerNode, nodeId);
                }
                break;

            case ActionMode.Disconnect:
                if (_rule.CanDisconnect(graph, playerNode, nodeId))
                {
                    return new DisconnectCommand(playerType, new EdgeData(playerNode, nodeId));
                }

                break;


            case ActionMode.Connect:
                if (_rule.CanConnect(graph, playerType, nodeId))
                {
                    return new ConnectCommand(playerType, new EdgeData(playerNode, nodeId));
                }
                break;
        }
        return null;
    }

    public void SetMode(ActionMode mode) => _mode = mode;
}
