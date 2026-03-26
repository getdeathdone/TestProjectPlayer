using Prototype.Input;
using Prototype.Config;
using Prototype.Player;
using UnityEngine;
using Zenject;

namespace Prototype.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float distance = 5.2f;
        [SerializeField] private float height = 1.9f;
        [SerializeField] private float shoulderOffsetX = 0.55f;
        [SerializeField] private float aimDistance = 2.6f;
        [SerializeField] private float aimHeight = 1.7f;
        [SerializeField] private float aimShoulderOffsetX = 0.35f;
        [SerializeField] private float followSmoothTime = 0.08f;
        [SerializeField] private float yawSmoothTime = 0.05f;
        [SerializeField] private float rotationSpeed = 220f;
        [SerializeField] private float joystickYawSpeed = 180f;
        [SerializeField] private float joystickPitchSpeed = 140f;
        [SerializeField] private float pitchMin = -20f;
        [SerializeField] private float pitchMax = 55f;
        [SerializeField] private bool requireRightMouse = true;
        [SerializeField] private float normalFov = 60f;
        [SerializeField] private float aimFov = 45f;
        [SerializeField] private float aimLerpSpeed = 10f;

        private PlayerService _playerService;
        private IInputService _inputService;
        private float _yaw;
        private float _pitch = 15f;
        private float _yawVelocity;
        private Vector3 _positionVelocity;
        private bool _isAiming;
        private bool _isMouseInputEnabled;

        [Inject]
        public void Construct(PlayerService playerService, IInputService inputService)
        {
            _playerService = playerService;
            _inputService = inputService;
        }

        public void SyncYawWithTarget()
        {
            var target = _playerService?.PlayerTransform;
            if (target != null)
            {
                _yaw = target.eulerAngles.y;
            }
        }

        private void LateUpdate()
        {
            var target = _playerService?.PlayerTransform;
            if (target == null)
            {
                return;
            }

            _isMouseInputEnabled = !Application.isMobilePlatform;
            var mouseAimPressed = _isMouseInputEnabled && UnityEngine.Input.GetMouseButton(GameplayConfig.Camera.AimMouseButton);
            _isAiming = mouseAimPressed || (_inputService != null && _inputService.AimPressed);
            var lookInput = _inputService != null ? _inputService.Look : Vector2.zero;
            var hasLookInput = lookInput.sqrMagnitude > GameplayConfig.Camera.LookInputSqrThreshold;
            var canRotate = !requireRightMouse || _isAiming || hasLookInput;
            if (canRotate)
            {
                var mouseX = _isMouseInputEnabled ? UnityEngine.Input.GetAxis("Mouse X") : 0f;
                var lookYaw = lookInput.x * joystickYawSpeed * Time.deltaTime;
                if (_isAiming || hasLookInput)
                {
                    target.Rotate(Vector3.up, (mouseX * rotationSpeed * Time.deltaTime) + lookYaw, Space.World);
                }

                var mouseY = _isMouseInputEnabled ? UnityEngine.Input.GetAxis("Mouse Y") : 0f;
                _pitch -= mouseY * rotationSpeed * Time.deltaTime;
                _pitch -= lookInput.y * joystickPitchSpeed * Time.deltaTime;
                _pitch = Mathf.Clamp(_pitch, pitchMin, pitchMax);
            }

            var targetYaw = target.eulerAngles.y;
            _yaw = Mathf.SmoothDampAngle(_yaw, targetYaw, ref _yawVelocity, yawSmoothTime);

            var currentDistance = _isAiming ? aimDistance : distance;
            var currentHeight = _isAiming ? aimHeight : height;
            var currentShoulder = _isAiming ? aimShoulderOffsetX : shoulderOffsetX;

            var rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            var shoulderOffset = rotation * new Vector3(currentShoulder, 0f, 0f);
            var backOffset = rotation * new Vector3(0f, 0f, -currentDistance);
            var targetPosition = target.position + Vector3.up * currentHeight + shoulderOffset;
            var desiredPosition = targetPosition + backOffset;

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _positionVelocity, followSmoothTime);
            transform.LookAt(targetPosition);

            var cameraComponent = GetComponent<UnityEngine.Camera>();
            if (cameraComponent != null)
            {
                var targetFov = _isAiming ? aimFov : normalFov;
                cameraComponent.fieldOfView = Mathf.Lerp(cameraComponent.fieldOfView, targetFov, aimLerpSpeed * Time.deltaTime);
            }
        }

    }
}
