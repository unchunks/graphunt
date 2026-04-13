using UnityEngine;
using UnityEngine.InputSystem;

public class TurnController: MonoBehaviour
{
    private GraphModel _graph;
    private RuleEngine _rule;
    private IStageRepository _stageRepo;

    private GameState _currentState;

    public void Initialize(GraphModel graph, RuleEngine rule, IStageRepository stageRepo)
    {
        _graph = graph;
        _rule = rule;
        _stageRepo = stageRepo;

        TransisionTo(GameState.Setup);
    }

    //private void Update()
    //{
    //    if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
    //        OnActionCompleted();
    //}

    public void TransisionTo(GameState nextState)
    {
        _currentState = nextState;
        Debug.Log($"State: {nextState}");

        switch (_currentState)
        {
            case GameState.Setup:      OnEnterSetup();      break;
            case GameState.RabbitTurn: OnEnterRabbitTurn(); break;
            case GameState.WolfTurn:   OnEnterWolfTurn();   break;
            case GameState.Result:     OnEnterResult();     break;
        }
    }

    private void OnEnterSetup()
    {
        _graph = _stageRepo.Load();

        TransisionTo(GameState.RabbitTurn);
    }

    private void OnEnterRabbitTurn()
    {
        Debug.Log("Rabbit's turn");
    }

    private void OnEnterWolfTurn() 
    {
        Debug.Log("Wolf's turn");
    }

    private void OnEnterResult() 
    {
        Debug.Log("Result");
    }

    public void OnActionCompleted()
    {
        if (CheckVictory(out GameResult result))
        {
            Debug.Log($"Game End: {result}");
            TransisionTo(GameState.Result);
            return;
        }

        TransisionTo(_currentState == GameState.RabbitTurn
            ? GameState.WolfTurn
            : GameState.RabbitTurn);
    }

    private bool CheckVictory(out GameResult result)
    {
        result = GameResult.None;
        return false;
    }
}

// 勝利結果の種類
public enum GameResult
{
    None,
    RabbitReachedGoal,   // ウサギがゴールに到達
    RabbitLooped,        // 千日手（ウサギ勝利）
    WolfCaught,          // オオカミがウサギを捕獲
}
