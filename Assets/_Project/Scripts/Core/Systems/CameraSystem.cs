using System.Collections.Generic;
using AE.Core.Types;
using JetBrains.Annotations;
using Unity.Cinemachine;
using UnityEngine;

namespace AE.Core.Systems
{
    [UsedImplicitly]
    public sealed class CameraSystem : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private CinemachineCamera pauseCinemachineCamera;
        [SerializeField] private CinemachineCamera firstPersonCinemachineCamera;
        [SerializeField] private CinemachineCamera inspectCinemachineCamera;

        private readonly Dictionary<GameMode, CinemachineCamera> _cinemachineCameras = new();

        private CinemachineCamera _activeCamera;

        public Transform MainCameraTransform => mainCamera.transform;

        private void Awake()
        {
            _cinemachineCameras[GameMode.Pause] = pauseCinemachineCamera;
            _cinemachineCameras[GameMode.Gameplay] = firstPersonCinemachineCamera;
            _cinemachineCameras[GameMode.Inspect] = inspectCinemachineCamera;
        }

        public void SwitchToCamera(GameMode gameMode)
        {
            if (_cinemachineCameras.TryGetValue(gameMode, out var cinemachineCamera))
            {
                if (_activeCamera != null)
                {
                    _activeCamera.enabled = false;
                }

                cinemachineCamera.enabled = true;
                _activeCamera = cinemachineCamera;
                Debug.Log($"Switched camera to {gameMode} mode");
            }
            else
            {
                Debug.LogWarning($"No camera found for game mode: {gameMode}");
            }
        }
    }
}