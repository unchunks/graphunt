using UnityEngine;

public class SetupState : IGameState
{
    private readonly IStageRepository _repo;

    public SetupState(IStageRepository repo)
    {
        _repo = repo;
    }

    public void OnEnter(TurnController controller)
    {
        Debug.Log("セットアップ開始");
        GraphModel graph = _repo.Load();
        controller.SetGraph(graph);
        controller.TransisionTo(new RabbitTurnState());
    }

    public void OnExit(TurnController controller)
    {

    }
}
