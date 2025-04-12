using AE.Core;
using AE.Core.Commands;
using AE.Core.GlobalGameState;
using AE.Core.Input;
using AE.Core.Systems;
using AE.Interactions.Manipulable;
using DG.Tweening;
using UnityEngine;
using VContainer;

namespace AE.Interactions.Inspectable
{
    public sealed class InspectSystem : MonoBehaviour
    {
        [Header("Inspection Settings")] [SerializeField]
        private Transform inspectedItemHolder;

        [SerializeField] private float transitionDuration = 0.5f;
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float zoomSpeed = 0.5f;
        [SerializeField] private float minZoom = 0.5f;
        [SerializeField] private float maxZoom = 2f;
        [SerializeField] private KeyCode takeItemKey = KeyCode.T;
        [SerializeField] private KeyCode toggleLightKey = KeyCode.L;

        [Header("Lighting")] [SerializeField] private GameObject inspectionLight;

        private InspectableItem _currentItem;
        private CameraSystem _cameraSystem;
        private CommandBus _commandBus;
        private ItemManipulationSystem _manipulationSystem;
        private InputSystem _inputSystem;

        private Vector3 _initialItemRotation;
        private Vector3 _initialItemScale;
        private float _currentZoom = 1f;
        private bool _isInspecting = false;
        private bool _isLightOn = false;
        private Transform _originalParent;

        [Inject]
        private void Construct(CameraSystem cameraSystem, CommandBus commandBus,
            ItemManipulationSystem manipulationSystem, InputSystem inputSystem)
        {
            _cameraSystem = cameraSystem;
            _commandBus = commandBus;
            _manipulationSystem = manipulationSystem;
            _inputSystem = inputSystem;

            // Subscribe to input events for rotation and zoom
            _inputSystem.Rotated += HandleItemRotation;
            _inputSystem.Zoomed += HandleItemZoom;
            _inputSystem.Exited += HandleExit;
        }

        private void OnDestroy()
        {
            if (_inputSystem != null)
            {
                _inputSystem.Rotated -= HandleItemRotation;
                _inputSystem.Zoomed -= HandleItemZoom;
                _inputSystem.Exited -= HandleExit;
            }
        }

        private void Awake()
        {
            if (inspectionLight != null)
            {
                inspectionLight.SetActive(false);
            }
        }

        private void Update()
        {
            if (!_isInspecting)
                return;

            // Exit inspection mode
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _commandBus.Execute(new ChangeGameStateCommand(GameMode.Gameplay));
                return;
            }

            // Take item after inspection
            if (Input.GetKeyDown(takeItemKey) && _currentItem != null)
            {
                TakeItemAfterInspection();
                return;
            }

            // Toggle inspection light
            if (Input.GetKeyDown(toggleLightKey) && inspectionLight != null)
            {
                _isLightOn = !_isLightOn;
                inspectionLight.SetActive(_isLightOn);

                // Position light near the inspected item
                if (_isLightOn)
                {
                    inspectionLight.transform.position = inspectedItemHolder.position + new Vector3(0, 1f, -1f);
                    inspectionLight.transform.LookAt(inspectedItemHolder);
                }
            }
        }

        public void SetItem(InspectableItem item)
        {
            _currentItem = item;
            _currentItem.Rigidbody.isKinematic = true;

            _originalParent = item.Transform.parent;
            _initialItemRotation = item.Transform.localEulerAngles;
            _initialItemScale = item.Transform.localScale;
            _currentZoom = 1f;
            _isInspecting = true;

            AlignCameraForInspection();

            item.Transform.SetParent(inspectedItemHolder, true);
            item.Transform.DOLocalMove(Vector3.zero, transitionDuration);
            item.Transform.DOLocalRotate(new Vector3(0, 0, 0), transitionDuration);

            // Change game state to inspection mode
            _commandBus.Execute(new ChangeGameStateCommand(GameMode.Inspect));
        }

        public void ExitInspection()
        {
            if (_currentItem == null)
                return;

            _isInspecting = false;

            _cameraSystem.AlignCameras(GameMode.Inspect, GameMode.Gameplay);

            _currentItem.Transform.SetParent(_originalParent, true);
            _currentItem.Transform.DOLocalRotate(_initialItemRotation, transitionDuration);
            _currentItem.Transform.DOScale(_initialItemScale, transitionDuration);

            _currentItem.Rigidbody.isKinematic = false;

            // Turn off the light if it's on
            if (_isLightOn && inspectionLight != null)
            {
                inspectionLight.SetActive(false);
                _isLightOn = false;
            }

            _currentItem = null;
            _originalParent = null;
        }

        private void TakeItemAfterInspection()
        {
            if (_currentItem == null)
                return;

            _isInspecting = false;

            // Return cameras to gameplay mode
            _cameraSystem.AlignCameras(GameMode.Inspect, GameMode.Gameplay);

            // Reset scale if zoomed
            _currentItem.Transform.DOScale(_initialItemScale, transitionDuration);

            // Turn off the light if it's on
            if (_isLightOn && inspectionLight != null)
            {
                inspectionLight.SetActive(false);
                _isLightOn = false;
            }

            // Transfer the item to the manipulation system
            _manipulationSystem.SetItemFromInspection(_currentItem, _originalParent);

            _currentItem = null;
            _originalParent = null;

            // Change back to gameplay state
            _commandBus.Execute(new ChangeGameStateCommand(GameMode.Gameplay));
        }

        private void AlignCameraForInspection()
        {
            var mainCamera = _cameraSystem.GetCamera(GameMode.Gameplay);
            var inspectCamera = _cameraSystem.GetCamera(GameMode.Inspect);

            var startPosition = mainCamera.transform.position;
            var startRotation = mainCamera.transform.rotation;

            var eulerAngles = startRotation.eulerAngles;
            eulerAngles.x = 0;
            var targetRotation = Quaternion.Euler(eulerAngles);

            inspectCamera.transform.position = startPosition;
            inspectCamera.transform.rotation = startRotation;

            inspectCamera.transform.DORotateQuaternion(targetRotation, transitionDuration);

            _cameraSystem.AlignCameras(GameMode.Gameplay, GameMode.Inspect);
        }

        private void HandleItemRotation(Vector2 mouseDelta)
        {
            if (!_isInspecting || _currentItem == null)
                return;

            _currentItem.Transform.Rotate(Vector3.up, -mouseDelta.x * rotationSpeed, Space.World);
            _currentItem.Transform.Rotate(Vector3.right, mouseDelta.y * rotationSpeed, Space.World);
        }

        private void HandleItemZoom(float scrollDelta)
        {
            if (!_isInspecting || _currentItem == null)
                return;

            _currentZoom = Mathf.Clamp(_currentZoom - scrollDelta * zoomSpeed, minZoom, maxZoom);
            _currentItem.Transform.DOScale(_initialItemScale * _currentZoom, 0.2f);
        }

        private void HandleExit()
        {
            if (_isInspecting)
            {
                ExitInspection();
                _commandBus.Execute(new ChangeGameStateCommand(GameMode.Gameplay));
            }
        }
    }
}