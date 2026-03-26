using Prototype.Config;
using Prototype.Input;
using Prototype.Signals;
using UnityEngine;
using Zenject;

namespace Prototype.Player
{
    [RequireComponent(typeof(Animator))]
    public class WeaponAnimatorBridge : MonoBehaviour
    {
        [Header("Blend")]
        [SerializeField] private int baseLayer = 0;
        [SerializeField] private float crossFadeSeconds = 0.12f;
        [SerializeField] private float equipLockSeconds = 0.2f;
        [SerializeField] private float moveEnterSpeed = 0.6f;
        [SerializeField] private float moveExitSpeed = 0.25f;

        private Animator _animator;
        private CharacterController _characterController;
        private IInputService _inputService;
        private SignalBus _signalBus;
        private bool _isSubscribed;
        private bool _hasWeapon;
        private bool _isMoving;
        private bool _isAiming;
        private float _lockedUntil;
        private int _currentStateHash = -1;

        [Inject]
        public void Construct(SignalBus signalBus, IInputService inputService)
        {
            _signalBus = signalBus;
            _inputService = inputService;
            TrySubscribe();
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _characterController = GetComponentInParent<CharacterController>();
            ValidateAnimatorSetup();
            UpdateState(true);
        }

        private void OnEnable()
        {
            TrySubscribe();
        }

        private void OnDisable()
        {
            if (_signalBus == null || !_isSubscribed)
            {
                return;
            }

            _signalBus.Unsubscribe<WeaponStateChangedSignal>(OnWeaponStateChanged);
            _isSubscribed = false;
        }

        private void Update()
        {
            if (_animator == null)
            {
                return;
            }

            var speed = GetPlanarSpeed();
            if (_isMoving)
            {
                _isMoving = speed > moveExitSpeed;
            }
            else
            {
                _isMoving = speed >= moveEnterSpeed;
            }
            var mouseAimPressed = !Application.isMobilePlatform && UnityEngine.Input.GetMouseButton(GameplayConfig.Camera.AimMouseButton);
            _isAiming = mouseAimPressed || (_inputService != null && _inputService.AimPressed);

            if (Time.time < _lockedUntil)
            {
                return;
            }

            UpdateState(false);
        }

        private void OnWeaponStateChanged(WeaponStateChangedSignal signal)
        {
            if (_animator == null)
            {
                return;
            }

            var becameArmed = !_hasWeapon && signal.HasWeapon;
            _hasWeapon = signal.HasWeapon;

            if (becameArmed && TryPlayState(AnimationHashConstant.StateHash.RifleEquip, true))
            {
                _lockedUntil = Time.time + equipLockSeconds;
            }
            else
            {
                UpdateState(false);
            }
        }

        private void TrySubscribe()
        {
            if (!isActiveAndEnabled || _signalBus == null || _isSubscribed)
            {
                return;
            }

            _signalBus.Subscribe<WeaponStateChangedSignal>(OnWeaponStateChanged);
            _isSubscribed = true;
        }

        private void ValidateAnimatorSetup()
        {
            if (_animator == null)
            {
                return;
            }
        }

        private void UpdateState(bool instant)
        {
            var targetStateHash = GetTargetStateHash();
            if (targetStateHash == 0)
            {
                return;
            }

            TryPlayState(targetStateHash, instant);
        }

        private int GetTargetStateHash()
        {
            if (_hasWeapon)
            {
                if (_isAiming)
                {
                    return _isMoving ? AnimationHashConstant.StateHash.RifleAimMove : AnimationHashConstant.StateHash.RifleAimIdle;
                }

                return _isMoving ? AnimationHashConstant.StateHash.RifleMove : AnimationHashConstant.StateHash.RifleIdle;
            }

            return _isMoving ? AnimationHashConstant.StateHash.Move : AnimationHashConstant.StateHash.Idle;
        }

        private bool TryPlayState(int stateHash, bool instant)
        {
            if (_animator == null || stateHash == 0)
            {
                return false;
            }

            if (!_animator.HasState(baseLayer, stateHash))
            {
                return false;
            }

            if (_currentStateHash == stateHash)
            {
                return true;
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
            return true;
        }

        private float GetPlanarSpeed()
        {
            if (_characterController == null)
            {
                return 0f;
            }

            var velocity = _characterController.velocity;
            velocity.y = 0f;
            return velocity.magnitude;
        }
    }
}
