using AE.Core.Systems;
using AE.Core.Utility;
using AE.Interactions.Trigger;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            EventManager.Notify(new GameStateEnterEvent(GameMode.GameOver));

            _cameraSystem.AlignCameras(GameMode.Gameplay, GameMode.GameOver);

            var gameOverCinemachine = _cameraSystem.GetCinemachine(GameMode.GameOver);
            var ghost = _utilities.Ghost;

            PlayJumpScareSequenceAsync(gameOverCinemachine, ghost).Forget();
        }

        //TODO: Create a sequence system to have more control
        private async UniTaskVoid PlayJumpScareSequenceAsync(CinemachineCamera gameOverCinemachine, Ghost ghost)
        {
            ghost.gameObject.SetActive(true);

            await UniTask.Delay(500);

            var direction = ghost.transform.position - gameOverCinemachine.transform.position;
            var targetRotation = Quaternion.LookRotation(direction);

            gameOverCinemachine.transform.DORotateQuaternion(targetRotation, 0.3f).SetEase(Ease.OutExpo);
            await UniTask.Delay(300);

            var player = _utilities.PlayerGameObject;
            var startPos = ghost.transform.position;
            var targetPos = player.transform.position;

            targetPos.y = startPos.y;

            var totalDistance = Vector3.Distance(startPos, targetPos);
            const float ghostSpeed = 5.0f;
            var remainingDistance = totalDistance;

            ghost.AudioSource.Play();

            while (remainingDistance > 1f)
            {
                var moveDistance = ghostSpeed * Time.deltaTime;
                var t = Mathf.Clamp01(moveDistance / remainingDistance);
                ghost.transform.position = Vector3.Lerp(ghost.transform.position, targetPos, t);

                remainingDistance = Vector3.Distance(ghost.transform.position, targetPos);

                await UniTask.Yield();
            }

            ghost.transform.position = targetPos;

            _utilities.FadeIn(0.5f);
            await UniTask.Delay(500);

            _utilities.ShowGameOverScreen();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public override void Exit()
        {
        }
    }
}