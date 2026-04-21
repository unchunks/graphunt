using System.Threading;
using UnityEngine;

public class WolfTurnState : IGameState
{
    public void OnEnter(TurnController controller)
    {
        Debug.Log("オオカミのターン");
        controller.BeginTurn(PlayerID.Wolf);

        // InputControllerの有効化
    }

    public void OnExit(TurnController controller)
    {
        // InputControllerの無効化
    }
}
