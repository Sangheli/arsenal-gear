namespace SangheliCore
{
    public interface IGameMode
    {
        void Init(IGameModeContainer gameModeContainer);
        void Start();
    }
}