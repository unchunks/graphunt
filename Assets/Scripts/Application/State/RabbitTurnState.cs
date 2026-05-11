using UnityEngine;

public class RabbitTurnState : IGameState
{
    public void OnEnter(TurnController controller)
    {
        Debug.Log("ウサギのターン");
        controller.BeginTurn(PlayerType.Rabbit);

        // InputControllerの有効化
    }

    public void OnExit(TurnController controller)
    {
        // InputControllerの無効化
    }
}
