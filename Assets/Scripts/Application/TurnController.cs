using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurnController : MonoBehaviour
{
    [SerializeField] private GraphView _graphView;

    private GraphModel _graph;
    private RuleEngine _rule;
    private IGameState _currentState;

    private IPlayerInputStrategy _rabbitStrategy;
    private IPlayerInputStrategy _wolfStrategy;
    private IPlayerInputStrategy _currentStrategy;
    private PlayerID _currentPlayerId;

    private CancellationTokenSource _turnCts;

    private CommandStack _commandStack = new();

    public void Initialize(
        RuleEngine rule,
        IStageRepository stageRepo,
        IPlayerInputStrategy rabbitStrategy,
        IPlayerInputStrategy wolfStrategy)
    {
        _rule = rule;

        _rabbitStrategy = rabbitStrategy;
        _wolfStrategy = wolfStrategy;

        TransisionTo(new SetupState(stageRepo));
    }

    public void TransisionTo(IGameState nextState)
    {
        CancelCurrentTurn();

        _currentState?.OnExit(this);
        _currentState = nextState;
        _currentState.OnEnter(this);
    }

    public void OnActionCompleted()
    {
        if (CheckVictory(out GameResult result))
        {
            Debug.Log($"Game End: {result}");
            TransisionTo(new ResultState(result));
            return;
        }

        TransisionTo(_currentState is RabbitTurnState
            ? new WolfTurnState()
            : new RabbitTurnState());
    }

    public void BeginTurn(PlayerID playerId)
    {
        _currentPlayerId = playerId;
        _currentStrategy = playerId == PlayerID.Rabbit ? _rabbitStrategy : _wolfStrategy;

        _turnCts = new CancellationTokenSource();
        RunTurnAsync(_turnCts.Token).Forget();
    }

    public void SetGraph(GraphModel graph)
    {
        _graph = graph;
        _graphView.Initialize(graph);
    }

    public GraphModel GetGraph() => _graph;
    public RuleEngine GetRule() => _rule;

    private async UniTaskVoid RunTurnAsync(CancellationToken ct)
    {
        IGameCommand command = await _currentStrategy.DecideActionAsync(_graph, _currentPlayerId, ct);

        if (ct.IsCancellationRequested) return;

        _commandStack.Execute(command, _graph);
        OnActionCompleted();
    }

    private bool CheckVictory(out GameResult result)
    {
        int rabbitPos = _graph.GetPlayerPosition(PlayerID.Rabbit);
        int wolfPos = _graph.GetPlayerPosition(PlayerID.Wolf);

        // ウサギがゴールに到達
        if (_graph.GetNodeType(rabbitPos) == NodeType.Goal)
        {
            result = GameResult.RabbitReachedGoal;
            return true;
        }

        // オオカミがウサギを捕獲
        if (rabbitPos == wolfPos)
        {
            result = GameResult.WolfCaught;
            return true;
        }

        // TODO: 千日手は第2週（ZobristHasher実装後）に追加
        result = GameResult.None;
        return false;
    }

    private void CancelCurrentTurn()
    {
        if (_turnCts != null && !_turnCts.IsCancellationRequested)
        {
            _turnCts.Cancel();
            _turnCts.Dispose();
            _turnCts = null;
        }
    }

    private void OnDestroy()
    {
        CancelCurrentTurn();
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            OnActionCompleted();
    }
}
