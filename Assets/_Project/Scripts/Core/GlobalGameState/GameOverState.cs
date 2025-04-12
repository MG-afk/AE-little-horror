using AE.Core.Systems;
using AE.Core.Utility;
using AE.Interactions.Trigger;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using Unity.Cinemachine;

namespace AE.Core.GlobalGameState
{
    [UsedImplicitly]
    public class GameOverState : GlobalGameStateMachine.State
    {
        private readonly CameraSystem _cameraSystem;
        private readonly Utilities _utilities;

        public GameOverState(CameraSystem cameraSystem, Utilities utilities)
        {
            _cameraSystem = cameraSystem;
            _utilities = utilities;
        }

        public override void Enter()
        {
            //TODO: Cursor System! 
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            EventManager.Notify(new GameStateEnterEvent(GameMode.Gameplay));

            _cameraSystem.AlignCameras(GameMode.Gameplay, GameMode.GameOver);

            var gameOverCinemachine = _cameraSystem.GetCinemachine(GameMode.GameOver);
            var ghost = _utilities.Ghost;

            PlayJumpScareSequenceAsync(gameOverCinemachine, ghost).Forget();
        }

        private async UniTaskVoid PlayJumpScareSequenceAsync(CinemachineCamera gameOverCinemachine, Ghost ghost)
        {
            ghost.gameObject.SetActive(true);
            
            await UniTask.Delay(500);

            gameOverCinemachine.LookAt = ghost.transform;

            await UniTask.Delay(300);

            var player = GameObject.FindGameObjectWithTag("Player");
            var startPos = ghost.transform.position;
            var targetPos = player.transform.position;

            targetPos.y = startPos.y;

            var duration = 1.0f;
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                ghost.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                await UniTask.Yield();
            }

            ghost.transform.position = targetPos;


            var audioSource = ghost.AudioSource;
            ghost.AudioSource.Play();

            var audioDuration = audioSource.clip.length;
            await UniTask.Delay((int)(audioDuration * 1000));

            _utilities.FadeOut(0.5f);
            await UniTask.Delay(5000);

            _utilities.ShowGameOverScreen();
        }

        public override void Exit()
        {
        }
    }
}