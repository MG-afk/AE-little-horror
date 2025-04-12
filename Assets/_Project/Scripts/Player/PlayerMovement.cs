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

        [Header("Movement Settings")]
        [SerializeField] private float speed = 5f;
        [SerializeField] private CharacterController characterController;
        
        [Header("Footstep Settings")]
        [SerializeField] private float footstepDistance = 2.0f; // Distance between footsteps
        [SerializeField, Range(0.8f, 1.2f)] private float minPitch = 0.8f;
        [SerializeField, Range(0.8f, 1.2f)] private float maxPitch = 1.2f;
        [SerializeField, Range(0.1f, 1.0f)] private float footstepVolume = 0.5f;

        private InputSystem _inputSystem;
        private Transform _cameraTransform;
        private AudioSystem _audioSystem;

        private Vector3 _direction;
        private Vector3 _velocity;
        private bool _isGrounded;
        private float _distanceTraveled = 0f;
        private Vector3 _lastPosition;
        private float _footstepCooldown = 0f;

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
            _lastPosition = transform.position;
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
            UpdateFootsteps();
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

        private void UpdateFootsteps()
        {
            // Handle cooldown timer
            if (_footstepCooldown > 0)
            {
                _footstepCooldown -= Time.deltaTime;
                return;
            }

            // Only play footsteps when grounded and moving
            if (!_isGrounded || _direction == Vector3.zero)
            {
                _distanceTraveled = 0;
                _lastPosition = transform.position;
                return;
            }

            // Calculate horizontal distance traveled since last update
            Vector3 currentPosition = transform.position;
            Vector3 movement = currentPosition - _lastPosition;
            movement.y = 0; // Only consider horizontal movement
            float movementMagnitude = movement.magnitude;
            
            // Prevent potential teleportation issues
            if (movementMagnitude > 10f)
            {
                _lastPosition = currentPosition;
                _distanceTraveled = 0;
                return;
            }
            
            _distanceTraveled += movementMagnitude;
            _lastPosition = currentPosition;

            // Play footstep sound when we've traveled enough distance
            if (_distanceTraveled >= footstepDistance)
            {
                PlayFootstepSound();
                _distanceTraveled = 0; // Reset distance counter
            }
        }

        private void PlayFootstepSound()
        {
            // Calculate footstep position slightly below the player
            Vector3 footstepPosition = transform.position - Vector3.up * 0.5f;
            
            // Play the sound
            var source = _audioSystem.PlaySound(SoundType.PlayerFootstep, footstepPosition, footstepVolume);
            
            if (source != null)
            {
                // Randomize pitch for variety
                source.pitch = Random.Range(minPitch, maxPitch);
                
                // Implement a safety cooldown to prevent rapid firing of sounds
                // Use 80% of the clip length as a minimum, to allow for some overlap
                float clipLength = source.clip?.length ?? 0.5f;
                _footstepCooldown = Mathf.Max(0.1f, clipLength * 0.8f);
            }
        }
    }
}