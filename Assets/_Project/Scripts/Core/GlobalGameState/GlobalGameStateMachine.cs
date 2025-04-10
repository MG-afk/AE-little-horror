using System.Collections.Generic;
using AE.Core.Event;
using AE.Core.StateMachines;
using AE.Core.Types;
using JetBrains.Annotations;
using UnityEngine;
using VContainer;

namespace AE.Core.GlobalGameState
{
    [UsedImplicitly]
    public class GlobalGameStateMachine : StateMachine<GlobalGameStateMachine.State>
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
            InspectState inspectState)
        {
            _states[GameMode.Gameplay] = gameplayState;
            _states[GameMode.Pause] = pauseState;
            _states[GameMode.Inspect] = inspectState;
        }

        public void ChangeState(GameMode gameMode)
        {
            ChangeState(_states[gameMode]);
        }

        public override void ChangeState(State newState)
        {
            base.ChangeState(newState);

            Debug.Log($"Entering {newState.GetType().Name} State");
        }
    }
}