using AE.Interactions.Inspectable;
using JetBrains.Annotations;
using UnityEngine;

namespace AE.Core.GlobalGameState
{
    [UsedImplicitly]
    public class InspectState : GlobalGameStateMachine.State
    {
        private readonly InspectSystem _inspectSystem;

        public InspectState(InspectSystem inspectSystem)
        {
            _inspectSystem = inspectSystem;
        }

        public override void Enter()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            EventManager.Notify(new GameStateEnterEvent(GameMode.Inspect));
        }

        public override void Exit()
        {
            _inspectSystem.ExitInspection();
        }
    }
}