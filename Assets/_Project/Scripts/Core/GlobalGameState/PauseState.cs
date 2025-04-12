using AE.Core.Event;
using AE.Core.Systems;
using AE.Pause;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace AE.Core.GlobalGameState
{
    [UsedImplicitly]
    public class PauseState : GlobalGameStateMachine.State
    {
        private readonly EventManager _eventManager;
        private readonly PauseView _pauseView;
        private readonly CameraSystem _cameraSystem;

        public PauseState(
            EventManager eventManager,
            PauseView pauseView,
            CameraSystem cameraSystem)
        {
            _eventManager = eventManager;
            _pauseView = pauseView;
            _cameraSystem = cameraSystem;
        }

        public override void Enter()
        {
            _eventManager.Notify(new GameStateEnterEvent(GameMode.Pause));

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            _cameraSystem.AlignCameras(GameMode.Gameplay, GameMode.Pause);
            _pauseView.ShowAsync().Forget();
        }

        public override void Exit()
        {
            _pauseView.HideAsync().Forget();
        }
    }
}