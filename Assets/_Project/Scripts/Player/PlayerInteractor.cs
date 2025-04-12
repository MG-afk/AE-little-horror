using AE.Core.Input;
using AE.Core.Systems;
using AE.Core.Utility;
using AE.Interactions;
using UnityEngine;
using VContainer;

namespace AE.Player
{
    public sealed class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private float interactionDistance = 3f;
        [SerializeField] private LayerMask interactableLayerMask;

        private InputSystem _inputSystem;
        private IInteractable _currentInteractable;
        private Camera _camera;

        [Inject]
        private void Construct(InputSystem inputSystem, CameraSystem cameraSystem)
        {
            _inputSystem = inputSystem;
            _camera = cameraSystem.MainCamera;
        }

        private void Awake()
        {
            _inputSystem.Interacted += OnInteract;
        }

        private void OnDestroy()
        {
            _inputSystem.Interacted -= OnInteract;
        }

        private void Update()
        {
            CheckForInteractables();
        }

        private void CheckForInteractables()
        {
            var foundInteractable = _camera.RaycastCenter(
                interactionDistance,
                interactableLayerMask,
                out var interactable);

            if (!foundInteractable)
            {
                _currentInteractable = null;
                return;
            }

            _currentInteractable = interactable;
        }

        private void OnInteract()
        {
            _currentInteractable?.Interact();
        }
    }
}