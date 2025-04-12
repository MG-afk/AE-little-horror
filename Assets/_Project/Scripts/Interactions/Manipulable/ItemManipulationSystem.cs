using AE.Core.GlobalGameState;
using AE.Core.Input;
using AE.Core.Systems;
using AE.Interactions.Inspectable;
using DG.Tweening;
using UnityEngine;
using VContainer;

namespace AE.Interactions.Manipulable
{
    public sealed class ItemManipulationSystem : MonoBehaviour
    {
        [Header("Carry Settings")] [SerializeField]
        private Transform itemHoldPoint;

        [SerializeField] private LayerMask placementSurfaces;
        [SerializeField] private LayerMask collisionLayers;

        [Header("Movement Settings")] [SerializeField]
        private float holdDistance = 2f;

        [SerializeField] private float minHoldDistance = 1f;
        [SerializeField] private float maxHoldDistance = 3f;
        [SerializeField] private float transitionDuration = 0.3f;
        [SerializeField] private float moveSensitivity = 0.1f;

        [Header("Physics Settings")] [SerializeField]
        private bool usePhysicsPlacement = true;

        [SerializeField] private float placementCheckRadius = 0.2f;
        [SerializeField] private float wallCheckDistance = 0.5f;
        [SerializeField] private float moveBackDistance = 0.2f;

        [Header("UI")] [SerializeField] private GameObject manipulationHintUI;

        private InspectableItem _currentHeldItem;
        private CameraSystem _cameraSystem;
        private InputSystem _inputSystem;
        private Transform _originalParent;
        private Vector3 _originalPosition;
        private Quaternion _originalRotation;
        private bool _isHolding;
        private bool _isPlacing;

        [Inject]
        private void Construct(CameraSystem cameraSystem, InputSystem inputSystem)
        {
            _cameraSystem = cameraSystem;
            _inputSystem = inputSystem;

            _inputSystem.Exited += HandleExit;
            _inputSystem.Zoomed += HandleZoom;
        }

        private void OnDestroy()
        {
            _inputSystem.Exited -= HandleExit;
            _inputSystem.Zoomed -= HandleZoom;
        }

        private void Start()
        {
            if (manipulationHintUI != null)
                manipulationHintUI.SetActive(false);
        }

        private void Update()
        {
            if (!_isHolding || _currentHeldItem == null)
                return;

            // Cancel manipulation with Escape
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CancelHolding();
                return;
            }

            // Toggle between holding and precise placement
            if (Input.GetKeyDown(KeyCode.R))
            {
                _isPlacing = !_isPlacing;
                UpdateHintUI();
            }

            if (_isPlacing)
            {
                HandlePlacementMode();
            }
            else
            {
                HandleHoldingMode();
            }

            // Place or drop the item
            if (Input.GetMouseButtonDown(0))
            {
                PlaceItem();
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                DropItem(true);
            }

            // Check for wall collisions to prevent clipping
            HandleWallCollisionPrevention();
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
            _originalPosition = item.Transform.position;
            _originalRotation = item.Transform.rotation;

            _currentHeldItem.Rigidbody.isKinematic = true;
            _isHolding = true;
            _isPlacing = false;

            _currentHeldItem.Transform.SetParent(itemHoldPoint, true);
            _currentHeldItem.Transform.DOMove(itemHoldPoint.position, transitionDuration);
            _currentHeldItem.Transform.DORotateQuaternion(itemHoldPoint.rotation, transitionDuration)
                .OnComplete(() =>
                {
                    if (manipulationHintUI == null)
                        return;

                    manipulationHintUI.SetActive(true);
                    DOVirtual.DelayedCall(5f, () =>
                    {
                        if (manipulationHintUI != null)
                            manipulationHintUI.SetActive(false);
                    });
                });
        }

        public void SetItemFromInspection(InspectableItem item, Transform originalParent)
        {
            if (item == null)
                return;

            _currentHeldItem = item;
            _originalParent = originalParent;
            _originalPosition = item.Transform.position;
            _originalRotation = item.Transform.rotation;

            _isHolding = true;
            _isPlacing = false;

            _currentHeldItem.Transform.SetParent(itemHoldPoint, true);
            _currentHeldItem.Transform.DOMove(itemHoldPoint.position, transitionDuration);

            UpdateHintUI(true);
        }

        private void HandleHoldingMode()
        {
            var camera = _cameraSystem.GetCamera(GameMode.Gameplay);
            _currentHeldItem.Transform.position = Vector3.Lerp(
                _currentHeldItem.Transform.position,
                camera.transform.position + camera.transform.forward * holdDistance,
                Time.deltaTime * 5f);
        }

        private void HandlePlacementMode()
        {
            RaycastHit hit;
            var camera = _cameraSystem.GetCamera(GameMode.Gameplay);

            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, maxHoldDistance,
                    placementSurfaces))
            {
                _currentHeldItem.Transform.position = Vector3.Lerp(
                    _currentHeldItem.Transform.position,
                    hit.point + hit.normal * 0.05f,
                    Time.deltaTime * 5f);

                if (Input.GetKey(KeyCode.LeftAlt))
                {
                    // Align with surface
                    var surfaceAlignment = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    _currentHeldItem.Transform.rotation = Quaternion.Lerp(
                        _currentHeldItem.Transform.rotation,
                        surfaceAlignment * Quaternion.Euler(0, camera.transform.eulerAngles.y, 0),
                        Time.deltaTime * 5f);
                }

                // Fine position adjustment with mouse when holding right-click
                if (Input.GetMouseButton(1))
                {
                    float moveX = Input.GetAxis("Mouse X") * moveSensitivity;
                    float moveY = Input.GetAxis("Mouse Y") * moveSensitivity;

                    Vector3 right = camera.transform.right;
                    Vector3 up = Vector3.Cross(right, hit.normal).normalized;

                    _currentHeldItem.Transform.position += right * moveX + up * moveY;
                }

                // Rotation adjustment with mouse wheel while holding Shift
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    float scrollInput = Input.GetAxis("Mouse ScrollWheel");
                    if (Mathf.Abs(scrollInput) > 0.01f)
                    {
                        _currentHeldItem.Transform.Rotate(Vector3.up, scrollInput * 100f, Space.World);
                    }
                }
            }
        }

        private void PlaceItem()
        {
            if (_currentHeldItem == null)
                return;

            // Check if placement is valid
            if (usePhysicsPlacement)
            {
                Collider[] colliders = Physics.OverlapSphere(
                    _currentHeldItem.Transform.position,
                    placementCheckRadius,
                    collisionLayers);

                // Check for collision with other objects (exclude the held item's colliders)
                bool isColliding = false;
                foreach (var collider in colliders)
                {
                    if (collider.gameObject != _currentHeldItem.gameObject &&
                        !collider.isTrigger &&
                        collider.gameObject.layer != LayerMask.NameToLayer("Player"))
                    {
                        // Placement spot is invalid
                        _currentHeldItem.Transform.DOShakePosition(0.2f, 0.05f, 10, 90, false);
                        isColliding = true;
                        break;
                    }
                }

                if (isColliding)
                    return;
            }

            // Place the item
            _currentHeldItem.Transform.SetParent(_originalParent, true);
            _currentHeldItem.Rigidbody.isKinematic = false;

            _isHolding = false;
            _currentHeldItem = null;

            UpdateHintUI(false);
        }

        private void DropItem(bool applyForce)
        {
            if (_currentHeldItem == null)
                return;

            _currentHeldItem.Transform.SetParent(_originalParent, true);
            _currentHeldItem.Rigidbody.isKinematic = false;

            if (applyForce)
            {
                // Add a small force in the forward direction when dropping
                var camera = _cameraSystem.GetCamera(GameMode.Gameplay);
                _currentHeldItem.Rigidbody.AddForce(
                    camera.transform.forward * 2f,
                    ForceMode.Impulse);
            }

            _isHolding = false;
            _currentHeldItem = null;

            UpdateHintUI(false);
        }

        private void CancelHolding()
        {
            if (_currentHeldItem == null)
                return;

            _currentHeldItem.Transform.SetParent(_originalParent, true);
            _currentHeldItem.Transform.DOMove(_originalPosition, transitionDuration);
            _currentHeldItem.Transform.DORotateQuaternion(_originalRotation, transitionDuration);
            _currentHeldItem.Rigidbody.isKinematic = false;

            _isHolding = false;
            _currentHeldItem = null;

            UpdateHintUI(false);
        }

        private void HandleWallCollisionPrevention()
        {
            if (_currentHeldItem == null || !_isHolding)
                return;

            var camera = _cameraSystem.GetCamera(GameMode.Gameplay);
            RaycastHit hit;

            // Check if we're about to hit a wall
            if (Physics.Raycast(camera.transform.position, camera.transform.forward,
                    out hit, holdDistance + wallCheckDistance, collisionLayers))
            {
                // If the hit is closer than our current hold distance
                if (hit.distance < holdDistance)
                {
                    // Calculate how much we need to move back
                    float moveBackAmount = (holdDistance - hit.distance) + moveBackDistance;

                    // Move the player back
                    var playerTransform = camera.transform.parent; // Assuming camera is child of player
                    if (playerTransform != null)
                    {
                        playerTransform.position -= camera.transform.forward * moveBackAmount;
                    }
                }
            }

            // Check if item itself would collide with something
            Collider[] colliders = Physics.OverlapSphere(
                _currentHeldItem.Transform.position,
                placementCheckRadius,
                collisionLayers);

            foreach (var collider in colliders)
            {
                if (collider.gameObject != _currentHeldItem.gameObject &&
                    !collider.isTrigger &&
                    collider.gameObject.layer != LayerMask.NameToLayer("Player"))
                {
                    // Push player back slightly
                    var playerTransform = camera.transform.parent;
                    if (playerTransform != null)
                    {
                        Vector3 pushDirection = (playerTransform.position - collider.transform.position).normalized;
                        playerTransform.position += pushDirection * moveBackDistance;
                    }

                    break;
                }
            }
        }

        private void HandleZoom(float scrollDelta)
        {
            if (!_isHolding || _isPlacing)
                return;

            holdDistance = Mathf.Clamp(holdDistance - scrollDelta, minHoldDistance, maxHoldDistance);
        }

        private void HandleExit()
        {
            if (_isHolding)
            {
                CancelHolding();
            }
        }

        private void UpdateHintUI(bool show = true)
        {
            if (manipulationHintUI == null)
                return;

            if (show && _isHolding)
            {
                manipulationHintUI.SetActive(true);
                DOVirtual.DelayedCall(5f, () =>
                {
                    if (manipulationHintUI != null)
                        manipulationHintUI.SetActive(false);
                });
            }
            else
            {
                manipulationHintUI.SetActive(false);
            }
        }
    }
}