using System.Collections.Generic;
using AE.Core.Event;
using AE.Core.StateMachines;
using JetBrains.Annotations;
using VContainer;
using VContainer.Unity;

namespace AE.Core.GlobalGameState
{
    [UsedImplicitly]
    public class GlobalGameStateMachine : StateMachine<GlobalGameStateMachine.State>, ITickable, IGlobalGameStateMachine
    {
        public abstract class State : IState
        {
            protected EventManager EventManager;

            [Inject]
            private void Construct(EventManager eventManager)
            {
                EventManager = eventManager;
            }

            public abstract void Enter();
            public abstract void Exit();

            public virtual void Update()
            {
            }
        }

        private readonly Dictionary<GameMode, State> _states = new();

        public GlobalGameStateMachine(
            GameplayState gameplayState,
            PauseState pauseState,
            InspectState inspectState,
            GameOverState gameOverState)
        {
            _states[GameMode.Gameplay] = gameplayState;
            _states[GameMode.Pause] = pauseState;
            _states[GameMode.Inspect] = inspectState;
            _states[GameMode.GameOver] = gameOverState;
        }

        public void ChangeState(GameMode gameMode)
        {
            ChangeState(_states[gameMode]);
        }

        public void Tick()
        {
            CurrentState?.Update();
        }
    }
}