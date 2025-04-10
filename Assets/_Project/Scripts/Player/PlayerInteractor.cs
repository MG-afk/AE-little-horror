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
        private bool _isInteracting;

        [Inject]
        private void Construct(InputSystem inputSystem)
        {
            _inputSystem = inputSystem;
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
            var foundInteractable = Raycaster.RaycastCenter(interactionDistance, interactableLayerMask,
                out IInteractable interactable);

            if (!foundInteractable)
            {
                _currentInteractable = null;
                return;
            }

            _currentInteractable = interactable;
        }

        private void OnInteract()
        {
            if (_currentInteractable == null || _isInteracting)
                return;

            _isInteracting = true;
            _currentInteractable.Interact();
            _isInteracting = false;
        }
    }
}