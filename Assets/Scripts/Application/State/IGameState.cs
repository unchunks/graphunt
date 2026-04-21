public interface IGameState
{
    void OnEnter(TurnController controller);
    void OnExit(TurnController controller);
}
