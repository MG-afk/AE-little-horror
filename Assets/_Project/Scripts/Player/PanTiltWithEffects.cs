using Unity.Cinemachine;
using UnityEngine;

namespace AE.Player
{
    [ExecuteAlways]
    [SaveDuringPlay]
    [AddComponentMenu("Cinemachine/Custom/Pan Tilt With Effects")]
    public class PanTiltWithEffects : CinemachinePanTilt
    {
        [Header("Sensitivity & Input")]
        public float sensitivity = 2f;

        [Tooltip("Link to Cinemachine Input Axis Controller (Input Provider)")]
        public CinemachineInputAxisController inputAxis;

        [Header("Snap Roll")]
        public float maxSnapRoll = 10f;
        public float snapSensitivity = 3f;
        public float snapRecoverySpeed = 3f;

        [Header("Tilt")]
        public float tiltAmount = 5f;
        public float tiltSpeed = 3f;
        [Tooltip("Enable tilt only when sprinting")]
        public bool tiltOnlyWhenSprinting = false;
        [HideInInspector] public bool IsSprinting = false;

        [Header("Reactive Sway")]
        public float swayIntensity = 3f;
        public float swaySpeed = 3f;

        [Header("Input Drag")]
        public float dragAmount = 2f;

        [Header("Camera Jitter")]
        public float jitterIntensity = 0.2f;
        public float jitterFrequency = 1.5f;

        // Internals
        private float _xRotation;
        private float _yaw;
        private float _currentRoll;
        private float _rollVelocity;
        private float _currentTilt;
        private float _tiltVelocity;
        private float _currentSway;
        private float _swayVelocity;
        private float _jitterTime;

        private Vector2 _smoothedInput;

        private CinemachineInputAxisController.Controller _xAxisController;
        private CinemachineInputAxisController.Controller _yAxisController;

        public override bool IsValid => FollowTarget != null;
        public override CinemachineCore.Stage Stage => CinemachineCore.Stage.Aim;

        protected override void OnEnable()
        {
            base.OnEnable();
            SetupInputControllers();
        }

        private void SetupInputControllers()
        {
            if (inputAxis == null) return;
            var controllers = inputAxis.Controllers;
            if (controllers is not { Count: >= 2 }) return;

            _xAxisController = controllers[0]; // Horizontal controller
            _yAxisController = controllers[1]; // Vertical controller
        }

        public override void MutateCameraState(ref CameraState state, float deltaTime)
        {
            if (!IsValid || inputAxis == null) return;

            if (_xAxisController == null || _yAxisController == null)
            {
                SetupInputControllers();
                if (_xAxisController == null || _yAxisController == null) return;
            }

            // Raw input
            float rawX = _xAxisController.InputValue * sensitivity;
            float rawY = -_yAxisController.InputValue * sensitivity;

            // Smoothed input (drag effect)
            Vector2 input = new(rawX, rawY);
            _smoothedInput = Vector2.Lerp(_smoothedInput, input, deltaTime * dragAmount);

            float mouseX = _smoothedInput.x;
            float mouseY = _smoothedInput.y;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -80f, 80f);
            _yaw += mouseX;

            // Snap roll (like a camera jerk)
            float targetRoll = -mouseX * snapSensitivity;
            targetRoll = Mathf.Clamp(targetRoll, -maxSnapRoll, maxSnapRoll);
            _currentRoll = Mathf.SmoothDamp(_currentRoll, targetRoll, ref _rollVelocity, 1f / snapRecoverySpeed);

            // Optional tilt on sprint
            float targetTilt = (tiltOnlyWhenSprinting && !IsSprinting) ? 0f : -tiltAmount;
            _currentTilt = Mathf.SmoothDamp(_currentTilt, targetTilt, ref _tiltVelocity, 1f / tiltSpeed);

            // Sway (reactive lean when turning)
            float targetSway = mouseX * swayIntensity;
            _currentSway = Mathf.SmoothDamp(_currentSway, targetSway, ref _swayVelocity, 1f / swaySpeed);
            Quaternion sway = Quaternion.Euler(0f, 0f, _currentSway);

            // Jitter (camera breathing)
            _jitterTime += deltaTime * jitterFrequency;
            float jitterX = (Mathf.PerlinNoise(_jitterTime, 0f) - 0.5f) * jitterIntensity;
            float jitterY = (Mathf.PerlinNoise(0f, _jitterTime) - 0.5f) * jitterIntensity;
            Quaternion jitter = Quaternion.Euler(jitterY, jitterX, 0f);

            // Final orientation
            Quaternion finalRot = Quaternion.Euler(_xRotation + _currentTilt, _yaw, _currentRoll);
            state.RawOrientation = jitter * sway * finalRot;
        }
    }
}
