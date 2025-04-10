using AE.Core.Types;
using JetBrains.Annotations;
using UnityEngine;

namespace AE.Core.GlobalGameState
{
    [UsedImplicitly]
    public class GameplayState : GlobalGameStateMachine.State
    {
        public override void Enter()
        {
            //TODO: Cursor System! 
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            EventManager.Notify(new GameStateEnterEvent(GameMode.Gameplay));
        }

        public override void Exit()
        {
            Debug.Log("Exiting Gameplay State");
        }
    }
}