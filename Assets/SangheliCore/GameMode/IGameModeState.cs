public interface IGameModeState
{
    GameState State { get; }
    void Execute();
}
