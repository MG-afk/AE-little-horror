using AE.Core.Types;
using UnityEngine;

namespace AE.Core.GlobalGameState
{
    public class GameplayState : GlobalGameStateMachine.State
    {
        public override void Enter()
        {
            Debug.Log("Entering Gameplay State");
            InputSystem.SwitchToGameMode(GameMode.Gameplay);
            CameraSystem.SwitchToCamera(GameMode.Gameplay);
            TimeSystem.ResumeTime();
            AudioSystem.SwitchToGameplayAudio();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public override void Exit()
        {
            Debug.Log("Exiting Gameplay State");
        }
    }
}