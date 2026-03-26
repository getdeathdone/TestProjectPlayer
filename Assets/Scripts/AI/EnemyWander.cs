using UnityEngine;
using Prototype.Config;

namespace Prototype.AI
{
    [RequireComponent(typeof(CharacterController))]
    public class EnemyWander : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 2.5f;
        [SerializeField] private float turnSpeed = 6f;
        [SerializeField] private float changeDirectionTime = 3f;
        [SerializeField] private float wanderRadius = 10f;
        
        [Header("Animation")]
        [SerializeField] private int baseLayer = 0;
        [SerializeField] private float moveEnterSpeed = 0.35f;
        [SerializeField] private float moveExitSpeed = 0.2f;
        [SerializeField] private float crossFadeSeconds = 0.12f;

        private CharacterController _controller;
        private Animator _animator;
        private Vector3 _origin;
        private Vector3 _direction;
        private float _timer;
        private float _verticalVelocity;
        private int _currentStateHash = -1;
        private bool _isMoving;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _animator = GetComponentInChildren<Animator>(true);
            _origin = transform.position;
            PickNewDirection();
            TryPlay(AnimationHashConstant.StateHash.Idle, true);
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= changeDirectionTime)
            {
                _timer = 0f;
                PickNewDirection();
            }

            ApplyGravity();
            var motion = _direction * moveSpeed + Vector3.up * _verticalVelocity;
            _controller.Move(motion * Time.deltaTime);
            UpdateAnimation();

            if (_direction.sqrMagnitude > GameplayConfig.Movement.DirectionSqrThreshold)
            {
                var targetRotation = Quaternion.LookRotation(_direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }

            var offset = transform.position - _origin;
            if (offset.sqrMagnitude > wanderRadius * wanderRadius)
            {
                _direction = (-offset).normalized;
            }
        }

        private void PickNewDirection()
        {
            var angle = Random.Range(0f, GameplayConfig.Movement.FullCircleDegrees);
            _direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)).normalized;
        }

        private void ApplyGravity()
        {
            if (_controller.isGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = GameplayConfig.CharacterControllerDefaults.GroundedVerticalVelocity;
            }

            _verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        private void UpdateAnimation()
        {
            if (_animator == null)
            {
                return;
            }

            var velocity = _controller.velocity;
            velocity.y = 0f;
            var speed = velocity.magnitude;

            if (_isMoving)
            {
                _isMoving = speed > moveExitSpeed;
            }
            else
            {
                _isMoving = speed >= moveEnterSpeed;
            }

            TryPlay(_isMoving ? AnimationHashConstant.StateHash.Move : AnimationHashConstant.StateHash.Idle, false);
        }

        private void TryPlay(int stateHash, bool instant)
        {
            if (_animator == null || stateHash == 0 || _currentStateHash == stateHash)
            {
                return;
            }

            if (!_animator.HasState(baseLayer, stateHash))
            {
                return;
            }

            if (instant)
            {
                _animator.Play(stateHash, baseLayer, 0f);
            }
            else
            {
                _animator.CrossFadeInFixedTime(stateHash, crossFadeSeconds, baseLayer);
            }

            _currentStateHash = stateHash;
        }
    }
}
