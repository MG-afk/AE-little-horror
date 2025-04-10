using AE.Core.Event;
using AE.Core.Types;

namespace AE.Core
{
    public class GameStateEnterEvent : IEvent
    {
        public GameMode GameMode { get; }

        public GameStateEnterEvent(GameMode gameMode)
        {
            GameMode = gameMode;
        }
    }
}