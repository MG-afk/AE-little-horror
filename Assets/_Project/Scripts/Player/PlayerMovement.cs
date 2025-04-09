using AE.Core.Systems;
using UnityEngine;
using VContainer;

namespace AE.Player
{
    public sealed class PlayerMovement : MonoBehaviour
    {
        private const float Gravity = 9.81f;

        [SerializeField] private float speed = 5f;
        [SerializeField] private CharacterController characterController;

        private InputSystem _inputSystem;
        private Transform _cameraTransform;
        private Vector3 _direction;
        private Vector3 _velocity;
        private bool _isGrounded;

        [Inject]
        private void Construct(InputSystem inputSystem, CameraSystem cameraSystem)
        {
            _inputSystem = inputSystem;
            _cameraTransform = cameraSystem.MainCameraTransform;
        }

        private void Awake()
        {
            _inputSystem.Moved += OnMove;
        }

        private void OnDestroy()
        {
            _inputSystem.Moved -= OnMove;
        }

        private void Update()
        {
            CheckGrounded();
            ApplyGravity();
            Move();
        }

        private void CheckGrounded()
        {
            _isGrounded = characterController.isGrounded;

            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = -0.5f;
            }
        }

        private void ApplyGravity()
        {
            _velocity.y -= Gravity * Time.deltaTime;
            characterController.Move(_velocity * Time.deltaTime);
        }

        private void Move()
        {
            if (_direction == Vector3.zero)
                return;

            var facingDirection = _cameraTransform.forward * _direction.z + _cameraTransform.right * _direction.x;

            facingDirection.y = 0;
            facingDirection.Normalize();

            characterController.Move(facingDirection * (speed * Time.deltaTime));
        }

        private void OnMove(Vector2 input)
        {
            _direction = new Vector3(input.x, 0, input.y).normalized;
        }
    }
}