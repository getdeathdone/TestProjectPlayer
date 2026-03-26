using Prototype.Input;
using Prototype.Config;
using UnityEngine;
using Zenject;

namespace Prototype.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5.5f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float gravity = -20f;
        [SerializeField] private bool allowKeyboard = true;

        private IInputService _inputService;
        private CharacterController _controller;
        private float _verticalVelocity;

        [Inject]
        public void Construct(IInputService inputService)
        {
            _inputService = inputService;
        }

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (_inputService == null)
            {
                return;
            }

            var input = _inputService.Move;
            if (allowKeyboard && input.sqrMagnitude < GameplayConfig.Movement.DirectionSqrThreshold)
            {
                var keyboard = new Vector2(UnityEngine.Input.GetAxisRaw("Horizontal"), UnityEngine.Input.GetAxisRaw("Vertical"));
                if (keyboard.sqrMagnitude > GameplayConfig.Movement.DirectionSqrThreshold)
                {
                    input = Vector2.ClampMagnitude(keyboard, GameplayConfig.Movement.MaxInputMagnitude);
                }
            }

            var desiredMove = new Vector3(input.x, 0f, input.y);

            if (desiredMove.sqrMagnitude > GameplayConfig.Movement.MaxInputMagnitude)
            {
                desiredMove.Normalize();
            }

            var worldMove = GetWorldMove(desiredMove);
            ApplyGravity();
            var motion = worldMove * moveSpeed + Vector3.up * _verticalVelocity;
            if (_controller == null)
            {
                return;
            }

            _controller.Move(motion * Time.deltaTime);

            if (worldMove.sqrMagnitude > GameplayConfig.Movement.DirectionSqrThreshold)
            {
                var targetRotation = Quaternion.LookRotation(worldMove, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        private Vector3 GetWorldMove(Vector3 input)
        {
            var camera = UnityEngine.Camera.main;
            if (camera == null)
            {
                return input;
            }

            var forward = camera.transform.forward;
            forward.y = 0f;
            forward.Normalize();

            var right = camera.transform.right;
            right.y = 0f;
            right.Normalize();

            return forward * input.z + right * input.x;
        }

        private void ApplyGravity()
        {
            if (_controller.isGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = GameplayConfig.CharacterControllerDefaults.GroundedVerticalVelocity;
            }

            _verticalVelocity += gravity * Time.deltaTime;
        }
    }
}
