using AE.Core.StateMachines;
using AE.Core.Systems;
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
            protected InputSystem InputSystem { get; private set; }
            protected CameraSystem CameraSystem { get; private set; }
            protected GameTimeSystem TimeSystem { get; private set; }
            protected AudioSystem AudioSystem { get; private set; }

            [Inject]
            private void Construct(
                InputSystem inputSystem,
                CameraSystem cameraSystem,
                GameTimeSystem timeSystem,
                AudioSystem audioSystem)
            {
                InputSystem = inputSystem;
                CameraSystem = cameraSystem;
                TimeSystem = timeSystem;
                AudioSystem = audioSystem;
            }

            public abstract void Enter();
            public abstract void Exit();

            public virtual void Update()
            {
            }
        }

        public override void ChangeState(State newState)
        {
            base.ChangeState(newState);

            Debug.Log($"Entering {nameof(newState)} State");
        }
    }
}