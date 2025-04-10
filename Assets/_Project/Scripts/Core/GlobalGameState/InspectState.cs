using AE.Core.Types;
using JetBrains.Annotations;
using UnityEngine;

namespace AE.Core.GlobalGameState
{
    [UsedImplicitly]
    public class InspectState : GlobalGameStateMachine.State
    {
        public override void Enter()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            EventManager.Notify(new GameStateEnterEvent(GameMode.Inspect));
        }

        public override void Exit()
        {
            Debug.Log("Exiting Inspect State");
        }
    }
}