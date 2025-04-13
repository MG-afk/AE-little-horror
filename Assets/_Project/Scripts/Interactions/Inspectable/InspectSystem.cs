using AE.Core;
using AE.Core.Commands;
using AE.Core.GlobalGameState;
using AE.Core.Input;
using AE.Core.Systems;
using AE.Core.Utility;
using AE.Interactions.Manipulable;
using AE.Riddle;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;

namespace AE.Interactions.Inspectable
{
    public sealed class InspectSystem : MonoBehaviour
    {
        [Header("Inspection Settings")] [SerializeField]
        private Transform inspectedItemHolder;

        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float zoomSpeed = 0.5f;
        [SerializeField] private float minZoom = 0.5f;
        [SerializeField] private float maxZoom = 2f;
        [SerializeField] private float cameraRotationDuration = 0.5f; // Duration for the camera rotation tween

        private InspectableItem _currentItem;
        private CameraSystem _cameraSystem;
        private CommandBus _commandBus;
        private InputSystem _inputSystem;
        private RiddleBlackboard _blackboard;
        private CinemachineCamera _cinemachineCamera;

        private Vector3 _initialItemScale;
        private float _currentZoom = 1f;
        private bool _isInspecting;
        private Transform _originalParent;
        private ManipulationHintUI _manipulationHintUI;

        private Vector2 _rotation;
        private float _zoom;

        [Inject]
        private void Construct(
            CameraSystem cameraSystem,
            CommandBus commandBus,
            InputSystem inputSystem,
            RiddleBlackboard blackboard,
            Utilities utilities)
        {
            _cameraSystem = cameraSystem;
            _commandBus = commandBus;
            _inputSystem = inputSystem;
            _blackboard = blackboard;
            _manipulationHintUI = utilities.ManipulationHintUI;
        }

        private void Start()
        {
            _cinemachineCamera = _cameraSystem.GetCinemachine(GameMode.Inspect);

            _inputSystem.Rotated += OnRotate;
            _inputSystem.Zoomed += OnZoom;
        }

        private void OnDestroy()
        {
            _inputSystem.Rotated -= OnRotate;
            _inputSystem.Zoomed -= OnZoom;
        }

        private void OnZoom(float zoom)
        {
            _zoom = zoom;
        }

        private void OnRotate(Vector2 rotation)
        {
            _rotation = rotation;
        }

        private void Update()
        {
            if (!_isInspecting || _currentItem == null)
                return;

            if (Input.GetMouseButton(0))
            {
                var mouseX = _rotation.x;
                var mouseY = _rotation.y;

                _currentItem.Transform.Rotate(Vector3.up, -mouseX * rotationSpeed, Space.Self);
                _currentItem.Transform.Rotate(Vector3.right, mouseY * rotationSpeed, Space.Self);
            }

            var scrollDelta = _zoom;
            if (Mathf.Abs(scrollDelta) > 0.01f)
            {
                _currentZoom = Mathf.Clamp(_currentZoom - scrollDelta * zoomSpeed, minZoom, maxZoom);
                _currentItem.Transform.localScale = _initialItemScale * _currentZoom;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ExitInspection();
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                var item = _currentItem;
                ExitInspection();
                _commandBus.Execute(new ChangeToManipulationModeCommand(item));
            }
        }

        public void SetItem(InspectableItem item)
        {
            if (_blackboard.CheckCondition(item.Condition))
            {
                _blackboard.SetValue(item.Key, RiddleConstant.Inspected);
            }

            _currentItem = item;
            _currentItem.Rigidbody.isKinematic = true;

            _originalParent = item.Transform.parent;
            _initialItemScale = item.Transform.localScale;
            _currentZoom = 1f;
            _isInspecting = true;

            item.Transform.SetParent(inspectedItemHolder, true);
            item.Transform.localPosition = Vector3.zero;
            item.Transform.localRotation = Quaternion.identity;
            _manipulationHintUI.ShowPlacementHints();

            _commandBus.Execute(new ChangeGameStateCommand(GameMode.Inspect));

            var currentRotation = _cinemachineCamera.transform.rotation.eulerAngles;
            var targetRotation = new Vector3(0f, currentRotation.y, currentRotation.z);

            _cinemachineCamera.transform.DORotate(targetRotation, cameraRotationDuration)
                .SetEase(Ease.OutQuint);
        }

        public void ExitInspection()
        {
            if (_currentItem == null)
                return;

            _isInspecting = false;

            _currentItem.Transform.SetParent(_originalParent, true);
            _currentItem.Transform.localScale = _initialItemScale;
            _currentItem.Rigidbody.isKinematic = false;

            _currentItem = null;
            _originalParent = null;
            _manipulationHintUI.Hide();

            _commandBus.Execute(new ChangeGameStateCommand(GameMode.Gameplay));
        }
    }
}