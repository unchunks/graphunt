using UnityEngine;

public class ResultState : IGameState
{
    private readonly GameResult _result;

    public ResultState(GameResult result)
    {
        _result = result;
    }

    public void OnEnter(TurnController controller)
    {
        Debug.Log($"ゲーム終了：{_result}");
    }

    public void OnExit(TurnController controller)
    {

    }
}
