using AE.Core.Event;
using AE.Core.Types;
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

        public PauseState(
            EventManager eventManager,
            PauseView pauseView)
        {
            _eventManager = eventManager;
            _pauseView = pauseView;
        }

        public override void Enter()
        {
            _eventManager.Notify(new GameStateEnterEvent(GameMode.Pause));

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            _pauseView.ShowAsync().Forget();
        }

        public override void Exit()
        {
            _pauseView.HideAsync().Forget();
        }
    }
}