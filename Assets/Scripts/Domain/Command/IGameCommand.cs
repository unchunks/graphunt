public interface IGameCommand
{
    void Execute(GraphModel graph);
    void Undo(GraphModel graph);
}
