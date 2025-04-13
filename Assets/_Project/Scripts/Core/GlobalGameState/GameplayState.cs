using AE.Core.Horror;
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
        private HorrorAudioService _horrorAudioService;

        [Inject]
        private void Construct(
            Utilities utilities,
            HorrorAudioService horrorAudioService)
        {
            _crosshair = utilities.Crosshair;
            _horrorAudioService = horrorAudioService;
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

        public override void Update()
        {
            _horrorAudioService?.Update();
        }
    }
}