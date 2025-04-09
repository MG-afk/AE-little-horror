using AE.Core.Types;
using UnityEngine;

namespace AE.Core.GlobalGameState
{
    public class PauseState : GlobalGameStateMachine.State
    {
        public override void Enter()
        {
            InputSystem.SwitchToGameMode(GameMode.Pause);
            CameraSystem.SwitchToCamera(GameMode.Pause);
            TimeSystem.PauseTime();
            AudioSystem.SwitchToPauseAudio();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public override void Exit()
        {
            Debug.Log("Exiting Pause State");
        }
    }
}