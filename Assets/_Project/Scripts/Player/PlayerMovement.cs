using AE.Core.Input;
using AE.Core.Systems;
using UnityEngine;
using VContainer;

namespace AE.Player
{
    public sealed class PlayerMovement : MonoBehaviour
    {
        private const float Gravity = 9.81f;
        private const float DefaultVelocityY = -.5f;

        [SerializeField] private float speed = 5f;
        [SerializeField] private CharacterController characterController;
        
        [SerializeField] private float groundCheckDistance = 0.5f;
        [SerializeField] private LayerMask groundLayer = -1;

        private InputSystem _inputSystem;
        private Transform _cameraTransform;

        private Vector3 _direction;
        private Vector3 _velocity;
        private bool _isGrounded;

        [Inject]
        private void Construct(
            InputSystem inputSystem,
            CameraSystem cameraSystem)
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
            var controllerGrounded = characterController.isGrounded;

            var raycastGrounded = false;
            var rayStart = transform.position + characterController.center;

            if (Physics.Raycast(rayStart, Vector3.down, out _, groundCheckDistance, groundLayer))
            {
                raycastGrounded = true;
            }

            _isGrounded = controllerGrounded || raycastGrounded;

            if (!_isGrounded) 
                return;
            
            if (!(_velocity.y < 0)) 
                return;
            
            _velocity.y = DefaultVelocityY;
        }

        private void ApplyGravity()
        {
            _velocity.y -= Gravity * Time.deltaTime;
            characterController.Move(_velocity * Time.deltaTime);
        }

        private void Move()
        {
            var forward = _cameraTransform.forward;
            var right = _cameraTransform.right;

            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            var moveDir = forward * _direction.z + right * _direction.x;
            var movement = moveDir.normalized * (speed * Time.deltaTime);
            characterController.Move(movement);
        }

        private void OnMove(Vector2 input)
        {
            _direction = new Vector3(input.x, 0, input.y).normalized;
        }
    }
}