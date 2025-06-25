using System.Collections.Generic;

namespace SangheliCore
{
    public interface IGameModeContainer
    {
        List<IGameModeState> GameModes { get; }
    }
}