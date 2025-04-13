using AE.Core.Utility;
using JetBrains.Annotations;
using UnityEngine;
using VContainer;

namespace AE.Core.GlobalGameState
{
    [UsedImplicitly]
    public class GameplayState : GlobalGameStateMachine.State
    {
        private Crosshair _crosshair;

        [Inject]
        private void Construct(Utilities utilities)
        {
            _crosshair = utilities.Crosshair;
        }

        public override void Enter()
        {
            _crosshair.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            EventManager.Notify(new GameStateEnterEvent(GameMode.Gameplay));
        }

        public override void Exit()
        {
            _crosshair.gameObject.SetActive(false);
        }
    }
}