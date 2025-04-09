using AE.Core.Types;
using UnityEngine;

namespace AE.Core.GlobalGameState
{
    public class InspectState : GlobalGameStateMachine.State
    {
        public InspectState()
        {
        }

        public override void Enter()
        {
            InputSystem.SwitchToGameMode(GameMode.Inspect);
            CameraSystem.SwitchToCamera(GameMode.Inspect);
            TimeSystem.SlowDownTime(0.1f);
            AudioSystem.SwitchToInspectionAudio();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public override void Exit()
        {
            Debug.Log("Exiting Inspect State");
        }
    }
}