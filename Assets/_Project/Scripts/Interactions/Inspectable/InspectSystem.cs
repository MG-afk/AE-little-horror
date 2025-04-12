using AE.Core;
using AE.Core.Commands;
using AE.Core.GlobalGameState;
using AE.Core.Input;
using AE.Core.Systems;
using AE.Riddle;
using UnityEditor.Experimental.GraphView;
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

        private InspectableItem _currentItem;
        private CameraSystem _cameraSystem;
        private CommandBus _commandBus;
        private InputSystem _inputSystem;
        private RiddleBlackboard _blackboard;

        private Vector3 _initialItemScale;
        private float _currentZoom = 1f;
        private bool _isInspecting = false;
        private Transform _originalParent;

        [Inject]
        private void Construct(
            CameraSystem cameraSystem,
            CommandBus commandBus,
            InputSystem inputSystem,
            RiddleBlackboard blackboard)
        {
            _cameraSystem = cameraSystem;
            _commandBus = commandBus;
            _inputSystem = inputSystem;
            _blackboard = blackboard;
        }

        private void Update()
        {
            if (!_isInspecting || _currentItem == null)
                return;

            if (Input.GetMouseButton(0))
            {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");
                _currentItem.Transform.Rotate(Vector3.up, -mouseX * rotationSpeed, Space.World);
                _currentItem.Transform.Rotate(Vector3.right, mouseY * rotationSpeed, Space.World);
            }

            var scrollDelta = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scrollDelta) > 0.01f)
            {
                _currentZoom = Mathf.Clamp(_currentZoom - scrollDelta * zoomSpeed, minZoom, maxZoom);
                _currentItem.Transform.localScale = _initialItemScale * _currentZoom;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ExitInspection();
                _commandBus.Execute(new ChangeGameStateCommand(GameMode.Gameplay));
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

            _commandBus.Execute(new ChangeGameStateCommand(GameMode.Inspect));
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
        }
    }
}