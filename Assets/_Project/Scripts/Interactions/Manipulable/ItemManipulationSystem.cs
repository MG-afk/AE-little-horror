using AE.Core.Input;
using AE.Core.Systems;
using AE.Core.Utility;
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
        [SerializeField] private float maxMoveSpeed = 10f;
        [SerializeField] private float collisionOffset = 0.1f;

        private InspectableItem _currentHeldItem;
        private CameraSystem _cameraSystem;
        private InputSystem _inputSystem;
        private Transform _originalParent;
        private bool _isHolding;
        private ManipulationHintUI _manipulationHintUI;
        private Vector3 _previousPosition;
        private float _itemRadius;
        private Camera _camera;

        [Inject]
        private void Construct(
            CameraSystem cameraSystem,
            InputSystem inputSystem,
            Utilities utilities)
        {
            _cameraSystem = cameraSystem;
            _inputSystem = inputSystem;
            _manipulationHintUI = utilities.ManipulationHintUI;
        }

        private void Awake()
        {
            _camera = _cameraSystem.MainCamera;
        }

        private void Update()
        {
            if (!_isHolding || _currentHeldItem == null)
                return;

            var desiredPosition = GetDesiredPositionWithCollisionCheck();

            if (_previousPosition != Vector3.zero)
            {
                var maxDistance = maxMoveSpeed * Time.deltaTime;
                if (Vector3.Distance(desiredPosition, _previousPosition) > maxDistance)
                {
                    desiredPosition =
                        _previousPosition + (desiredPosition - _previousPosition).normalized * maxDistance;
                }
            }

            _currentHeldItem.Transform.position = desiredPosition;
            _previousPosition = desiredPosition;

            var scrollDelta = Input.GetAxis("Mouse ScrollWheel");
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

        private Vector3 GetDesiredPositionWithCollisionCheck()
        {
            var cameraPosition = _camera.transform.position;
            var forward = _camera.transform.forward;
            var targetPosition = cameraPosition + forward * holdDistance;

            if (Physics.Raycast(cameraPosition, forward, out var hit, holdDistance, collisionLayers))
            {
                return hit.point - forward * (_itemRadius + collisionOffset);
            }

            var colliders = new Collider[32];
            var size = Physics.OverlapSphereNonAlloc(targetPosition, _itemRadius, colliders, collisionLayers);

            if (size <= 0)
                return targetPosition;

            for (var index = 0; index < size; index++)
            {
                var collider = colliders[index];
                if (collider.gameObject == _currentHeldItem.gameObject)
                    continue;

                var closestPoint = collider.ClosestPoint(targetPosition);
                var distance = Vector3.Distance(closestPoint, targetPosition);

                if (distance < _itemRadius)
                {
                    var direction = (targetPosition - closestPoint).normalized;
                    targetPosition = closestPoint + direction * (_itemRadius + collisionOffset);
                }
            }

            return targetPosition;
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
            _manipulationHintUI.ShowHoldingHints();
            _previousPosition = Vector3.zero;

            _itemRadius = _currentHeldItem.GetComponent<Collider>() != null
                ? _currentHeldItem.GetComponent<Collider>().bounds.extents.magnitude
                : 0.5f;

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
            _manipulationHintUI.Hide();
        }

        private void DropItem()
        {
            if (_currentHeldItem == null)
                return;

            _currentHeldItem.Transform.SetParent(_originalParent, true);
            _currentHeldItem.Rigidbody.isKinematic = false;

            _currentHeldItem.Rigidbody.AddForce(_camera.transform.forward * 2f, ForceMode.Impulse);

            _isHolding = false;
            _currentHeldItem = null;
            _manipulationHintUI.Hide();
        }

        private void CancelHolding()
        {
            if (_currentHeldItem == null)
                return;

            _currentHeldItem.Transform.SetParent(_originalParent, true);
            _currentHeldItem.Rigidbody.isKinematic = false;

            _isHolding = false;
            _currentHeldItem = null;
            _manipulationHintUI.Hide();
        }
    }
}