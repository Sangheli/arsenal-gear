using System.Collections.Generic;
using SangheliCore;

namespace SangheliCode
{
    public class GameModeContainer : IGameModeContainer
    {
        private readonly List<IGameModeState> _gameModes = new()
        {

        };

        public List<IGameModeState> GameModes => _gameModes;
    }
}