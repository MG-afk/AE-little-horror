using AE.Core.Event;
using AE.Core.GlobalGameState;

namespace AE.Core
{
    public readonly struct GameStateEnterEvent : IEvent
    {
        public GameMode GameMode { get; }

        public GameStateEnterEvent(GameMode gameMode)
        {
            GameMode = gameMode;
        }
    }
}