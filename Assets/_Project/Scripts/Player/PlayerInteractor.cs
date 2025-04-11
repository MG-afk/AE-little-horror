using AE.Core.Systems;
using AE.Core.Utility;
using AE.Interactions;
using Cysharp.Threading.Tasks;
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
        private Utilities _utilities;

        [Inject]
        private void Construct(
            InputSystem inputSystem,
            Utilities utilities)
        {
            _inputSystem = inputSystem;
            _utilities = utilities;
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
            if (_currentInteractable == null)
                return;

            _currentInteractable.Interact();
            _utilities.SimplifyDialogueView.Show(_currentInteractable.Text).Forget();
        }
    }
}