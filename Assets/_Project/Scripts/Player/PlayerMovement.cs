using AE.Core.Input;
using AE.Core.Systems;
using AE.Core.Systems.Audio;
using UnityEngine;
using VContainer;

namespace AE.Player
{
    public sealed class PlayerMovement : MonoBehaviour
    {
        private const float Gravity = 9.81f;
        private const float GravityScale = 1f;
        private const float DefaultVelocityY = -.5f;

        [Header("Movement Settings")] [SerializeField]
        private float speed = 5f;

        [SerializeField] private CharacterController characterController;

        [Header("Footstep Settings")] [SerializeField]
        private float stepInterval = 0.5f;

        [SerializeField, Range(0.8f, 1.2f)] private float minPitch = 0.8f;
        [SerializeField, Range(0.8f, 1.2f)] private float maxPitch = 1.2f;
        [SerializeField, Range(0.1f, 1.0f)] private float footstepVolume = 0.5f;

        [Header("Ground Detection")] [SerializeField]
        private float groundCheckDistance = 0.5f;

        [SerializeField] private float airborneGracePeriod = 0.25f;
        [SerializeField] private LayerMask groundLayer = -1;

        private InputSystem _inputSystem;
        private Transform _cameraTransform;
        private AudioSystem _audioSystem;

        private Vector3 _direction;
        private Vector3 _velocity;
        private bool _isGrounded;
        private bool _wasMoving;
        private float _nextFootstepTime;
        private float _lastGroundedTime;

        [Inject]
        private void Construct(
            InputSystem inputSystem,
            CameraSystem cameraSystem,
            AudioSystem audioSystem)
        {
            _inputSystem = inputSystem;
            _cameraTransform = cameraSystem.MainCameraTransform;
            _audioSystem = audioSystem;
        }

        private void Awake()
        {
            _inputSystem.Moved += OnMove;
            _nextFootstepTime = 0f;
            _wasMoving = false;
            _lastGroundedTime = 0f;
        }

        private void OnDestroy()
        {
            _inputSystem.Moved -= OnMove;
        }

        private void Update()
        {
            CheckGrounded();
            ApplyGravity();
            var movement = Move();
            UpdateFootsteps(movement);
        }

        private void CheckGrounded()
        {
            var controllerGrounded = characterController.isGrounded;

            var raycastGrounded = false;
            var rayStart = transform.position + characterController.center;

            if (Physics.Raycast(rayStart, Vector3.down, out var _, groundCheckDistance, groundLayer))
            {
                raycastGrounded = true;
            }

            _isGrounded = controllerGrounded || raycastGrounded;

            if (_isGrounded)
            {
                _lastGroundedTime = Time.time;

                if (_velocity.y < 0)
                {
                    _velocity.y = DefaultVelocityY;
                }
            }
        }

        private bool IsEffectivelyGrounded()
        {
            return _isGrounded || (Time.time - _lastGroundedTime < airborneGracePeriod);
        }

        private void ApplyGravity()
        {
            _velocity.y -= Gravity * GravityScale * Time.deltaTime;
            characterController.Move(_velocity * Time.deltaTime);
        }

        private Vector3 Move()
        {
            var isMovingNow = _direction != Vector3.zero;

            if (!isMovingNow)
            {
                if (_wasMoving)
                {
                    _nextFootstepTime = 0f;
                }

                _wasMoving = false;
                return Vector3.zero;
            }

            _wasMoving = true;
            var forward = _cameraTransform.forward;
            var right = _cameraTransform.right;

            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            var moveDir = forward * _direction.z + right * _direction.x;
            var movement = moveDir.normalized * (speed * Time.deltaTime);
            characterController.Move(movement);

            return movement;
        }

        private void OnMove(Vector2 input)
        {
            _direction = new Vector3(input.x, 0, input.y).normalized;
        }

        private void UpdateFootsteps(Vector3 movement)
        {
            if (!IsEffectivelyGrounded() || !_wasMoving)
                return;

            if (Time.time >= _nextFootstepTime)
            {
                PlayFootstepSound();

                var currentSpeed = movement.magnitude / Time.deltaTime;
                var speedFactor = currentSpeed / speed;
                var adjustedInterval = Mathf.Lerp(stepInterval * 1.5f, stepInterval * 0.7f, speedFactor);

                _nextFootstepTime = Time.time + adjustedInterval;
            }
        }

        private void PlayFootstepSound()
        {
            var footstepPosition = transform.position - Vector3.up * 0.5f;

            var source = _audioSystem.PlaySound(SoundType.PlayerFootstep, footstepPosition, footstepVolume);

            if (source != null)
            {
                source.pitch = Random.Range(minPitch, maxPitch);
            }
        }
    }
}