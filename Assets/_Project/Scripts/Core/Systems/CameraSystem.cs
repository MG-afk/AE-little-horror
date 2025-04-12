using System.Collections.Generic;
using AE.Core.Event;
using AE.Core.GlobalGameState;
using JetBrains.Annotations;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;

namespace AE.Core.Systems
{
    [UsedImplicitly]
    public sealed class CameraSystem : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private CinemachineCamera pauseCinemachineCamera;
        [SerializeField] private CinemachineCamera firstPersonCinemachineCamera;
        [SerializeField] private CinemachineCamera inspectCinemachineCamera;
        [SerializeField] private CinemachineCamera gameOverCinemachineCamera;

        private readonly Dictionary<GameMode, CinemachineCamera> _cinemachineCameras = new();

        private EventManager _eventManager;
        private CinemachineCamera _activeCamera;

        public Transform MainCameraTransform => mainCamera.transform;
        public Camera MainCamera => mainCamera;

        [Inject]
        private void Construct(EventManager eventManager)
        {
            _eventManager = eventManager;
        }

        private void Awake()
        {
            _cinemachineCameras[GameMode.Pause] = pauseCinemachineCamera;
            _cinemachineCameras[GameMode.Gameplay] = firstPersonCinemachineCamera;
            _cinemachineCameras[GameMode.Inspect] = inspectCinemachineCamera;
            _cinemachineCameras[GameMode.GameOver] = gameOverCinemachineCamera;

            _eventManager.Subscribe<GameStateEnterEvent>(SwitchToCamera);
        }

        private void OnDestroy()
        {
            _eventManager.Unsubscribe<GameStateEnterEvent>(SwitchToCamera);
        }

        private void SwitchToCamera(in GameStateEnterEvent gameStateEnterEvent)
        {
            var gameMode = gameStateEnterEvent.GameMode;
            if (_cinemachineCameras.TryGetValue(gameMode, out var cinemachineCamera))
            {
                if (_activeCamera != null)
                {
                    _activeCamera.enabled = false;
                }

                cinemachineCamera.enabled = true;
                _activeCamera = cinemachineCamera;
            }
            else
            {
                Debug.LogWarning($"No camera found for game mode: {gameMode}");
            }
        }

        public void AlignCameras(GameMode from, GameMode to)
        {
            var fromCamera = _cinemachineCameras[from];
            var toCamera = _cinemachineCameras[to];

            toCamera.transform.position = fromCamera.transform.position;
            toCamera.transform.rotation = fromCamera.transform.rotation;
        }

        public CinemachineCamera GetCinemachine(GameMode mode)
        {
            return _cinemachineCameras[mode];
        }
    }
}