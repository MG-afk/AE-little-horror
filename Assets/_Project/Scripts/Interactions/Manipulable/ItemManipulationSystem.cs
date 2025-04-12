using AE.Core.GlobalGameState;
using AE.Core.Input;
using AE.Core.Systems;
using AE.Interactions.Inspectable;
using UnityEngine;
using VContainer;

namespace AE.Interactions.Manipulable
{
    public sealed class ItemManipulationSystem : MonoBehaviour
    {
        [Header("Carry Settings")] [SerializeField]
        private float holdDistance = 2f;

        [SerializeField] private float minHoldDistance = 1f;
        [SerializeField] private float maxHoldDistance = 3f;
        [SerializeField] private LayerMask collisionLayers;

        private InspectableItem _currentHeldItem;
        private CameraSystem _cameraSystem;
        private InputSystem _inputSystem;
        private Transform _originalParent;
        private bool _isHolding;

        [Inject]
        private void Construct(CameraSystem cameraSystem, InputSystem inputSystem)
        {
            _cameraSystem = cameraSystem;
            _inputSystem = inputSystem;
        }

        private void Update()
        {
            if (!_isHolding || _currentHeldItem == null)
                return;

            var camera = _cameraSystem.GetCamera(GameMode.Gameplay);
            _currentHeldItem.Transform.position = camera.transform.position + camera.transform.forward * holdDistance;

            float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scrollDelta) > 0.01f)
            {
                holdDistance = Mathf.Clamp(holdDistance - scrollDelta, minHoldDistance, maxHoldDistance);
            }

            if (Input.GetMouseButtonDown(0))
            {
                PlaceItem();
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                DropItem();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                CancelHolding();
            }
        }

        public bool CanPickupItem(InspectableItem item)
        {
            return !_isHolding && item != null;
        }

        public void PickupItem(InspectableItem item)
        {
            if (_isHolding || item == null)
                return;

            _currentHeldItem = item;
            _originalParent = item.Transform.parent;
            _currentHeldItem.Rigidbody.isKinematic = true;
            _isHolding = true;

            _currentHeldItem.Transform.SetParent(null);
        }

        private void PlaceItem()
        {
            if (_currentHeldItem == null)
                return;

            _currentHeldItem.Transform.SetParent(_originalParent, true);
            _currentHeldItem.Rigidbody.isKinematic = false;

            _isHolding = false;
            _currentHeldItem = null;
        }

        private void DropItem()
        {
            if (_currentHeldItem == null)
                return;

            _currentHeldItem.Transform.SetParent(_originalParent, true);
            _currentHeldItem.Rigidbody.isKinematic = false;

            var camera = _cameraSystem.GetCamera(GameMode.Gameplay);
            _currentHeldItem.Rigidbody.AddForce(camera.transform.forward * 2f, ForceMode.Impulse);

            _isHolding = false;
            _currentHeldItem = null;
        }

        private void CancelHolding()
        {
            if (_currentHeldItem == null)
                return;

            _currentHeldItem.Transform.SetParent(_originalParent, true);
            _currentHeldItem.Rigidbody.isKinematic = false;

            _isHolding = false;
            _currentHeldItem = null;
        }
    }
}