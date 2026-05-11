using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TurnController : MonoBehaviour
{
    [SerializeField] private GraphView _graphView;

    private GraphModel _graph;
    private RuleEngine _rule;
    private IGameState _currentState;

    private ZobristHasher _hasher;
    private GameStateHistory _history;

    private IPlayerInputStrategy _rabbitStrategy;
    private IPlayerInputStrategy _wolfStrategy;
    private IPlayerInputStrategy _currentStrategy;
    private PlayerType _currentPlayerType;

    private CancellationTokenSource _turnCts;

    private CommandStack _commandStack = new();


    public void Initialize(
        RuleEngine rule,
        ZobristHasher hasher,
        IStageRepository stageRepo,
        IPlayerInputStrategy rabbitStrategy,
        IPlayerInputStrategy wolfStrategy)
    {
        _rule = rule;
        _hasher = hasher;
        _history = new GameStateHistory();


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

    // TODO: 現在の経過ターンを更新する
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

    public void BeginTurn(PlayerType playerType)
    {
        _currentPlayerType = playerType;
        _currentStrategy = playerType == PlayerType.Rabbit ? _rabbitStrategy : _wolfStrategy;

        _turnCts = new CancellationTokenSource();
        RunTurnAsync(_turnCts.Token).Forget();
    }

    public void SetGraph(GraphModel graph)
    {
        _graph = graph;

        // グラフ確定後に初期ハッシュを計算
        _graph.CurrentHash = _hasher.ComputeFullHash(_graph, isWolfTurn: false);

        // 初期盤面を履歴に登録
        _history.TryRegister(_graph.CurrentHash);

        _graphView.Initialize(_graph);
    }

    public void Undo()
    {
        if (!_commandStack.CanUndo) return;

        CancelCurrentTurn();

        _commandStack.Undo(_graph);

        _history.PopLast();

        TransisionTo(_currentState is RabbitTurnState ? new WolfTurnState() : new RabbitTurnState());
    }

    public void Redo()
    {
        if (!_commandStack.CanRedo) return;

        CancelCurrentTurn();

        _commandStack.Redo(_graph);

        TransisionTo(_currentState is RabbitTurnState ? new WolfTurnState() : new RabbitTurnState());
    }

    private async UniTaskVoid RunTurnAsync(CancellationToken ct)
    {
        IGameCommand command = await _currentStrategy.DecideActionAsync(_graph, _currentPlayerType, ct);

        if (ct.IsCancellationRequested) return;

        _commandStack.Execute(command, _graph);

        OnActionCompleted();
    }

    private bool CheckVictory(out GameResult result)
    {
        int rabbitPos = _graph.GetPlayerPosition(PlayerID.Rabbit);
        int wolfAPos = _graph.GetPlayerPosition(PlayerID.WolfA);
        int wolfBPos = _graph.GetPlayerPosition(PlayerID.WolfB);

        // ウサギがゴールに到達
        if (_graph.GetNodeType(rabbitPos) == NodeType.Goal)
        {
            result = GameResult.RabbitReachedGoal;
            return true;
        }

        // オオカミがウサギを捕獲
        if (rabbitPos == wolfAPos || rabbitPos == wolfBPos)
        {
            result = GameResult.WolfCaught;
            return true;
        }

        // 千日手の判定
        if (_history.TryRegister(_graph.CurrentHash))
        { result = GameResult.RabbitLooped; return true; }

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
        {
            // 現在アクティブなシーンの情報を取得
            Scene currentScene = SceneManager.GetActiveScene();

            // そのシーンの名前を使って再度ロードする
            SceneManager.LoadScene(currentScene.name);
        }
    }
}
