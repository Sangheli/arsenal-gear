using System.Linq;
using SangheliCore;

namespace SangheliCode
{
    public class GameModeManager : IGameMode
    {
        private IGameModeContainer _gameModeContainer;
        
        public void Init(IGameModeContainer gameModeContainer)
        {
            _gameModeContainer = gameModeContainer;
            // G.I.GameStateUpdate += OnGameState;
        }

        public void Start()
        {
            // G.I.GameStateUpdate.Invoke(G.I.StartGameState);
        }

        private void OnGameState(GameState state)
        {
            var gameModeState = _gameModeContainer.GameModes.FirstOrDefault(x => x.State == state);
            gameModeState?.Execute();
        }
    }
}