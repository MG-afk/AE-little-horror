using AE.Core.Systems;
using AE.Interactions.Inspectable;
using JetBrains.Annotations;
using UnityEngine;

namespace AE.Core.GlobalGameState
{
    [UsedImplicitly]
    public class InspectState : GlobalGameStateMachine.State
    {
        private readonly InspectSystem _inspectSystem;
        private readonly CameraSystem _cameraSystem;

        public InspectState(InspectSystem inspectSystem, CameraSystem cameraSystem)
        {
            _inspectSystem = inspectSystem;
            _cameraSystem = cameraSystem;
        }

        public override void Enter()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            EventManager.Notify(new GameStateEnterEvent(GameMode.Inspect));
            _cameraSystem.AlignCameras(GameMode.Gameplay, GameMode.Inspect);
        }

        public override void Exit()
        {
            _inspectSystem.ExitInspection();
        }
    }
}