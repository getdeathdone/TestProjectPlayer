using Prototype.Input;
using Prototype.Signals;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Zenject;

namespace Prototype.UI
{
    public class UIController : MonoBehaviour
    {
        [Header("Joysticks")]
        [SerializeField] private FixedJoystick moveJoystick;
        [SerializeField] private FixedJoystick lookJoystick;

        [Header("Buttons")]
        [SerializeField] private Button pickUpButton;
        [SerializeField] private Button aimButton;
        [SerializeField] private Button fireButton;

        [Header("Optional Hint")]
        [SerializeField] private GameObject pickupHintRoot;

        private IInputService _inputService;
        private SignalBus _signalBus;
        private bool _holdButtonsBound;

        [Inject]
        public void Construct(IInputService inputService, SignalBus signalBus)
        {
            _inputService = inputService;
            _signalBus = signalBus;
        }

        private void OnEnable()
        {
            if (_signalBus != null)
            {
                _signalBus.Subscribe<WeaponStateChangedSignal>(OnWeaponStateChanged);
            }

            if (pickUpButton != null)
            {
                pickUpButton.onClick.AddListener(OnPickUpClicked);
                pickUpButton.gameObject.SetActive(false);
            }

            if (!_holdButtonsBound)
            {
                BindHoldButton(aimButton, OnAimButtonHoldChanged);
                BindHoldButton(fireButton, OnFireButtonHoldChanged);
                _holdButtonsBound = true;
            }

            if (aimButton != null)
            {
                aimButton.gameObject.SetActive(false);
            }

            if (fireButton != null)
            {
                fireButton.gameObject.SetActive(false);
            }

            if (pickupHintRoot != null)
            {
                pickupHintRoot.SetActive(false);
            }
        }

        private void OnDisable()
        {
            if (_signalBus != null)
            {
                _signalBus.Unsubscribe<WeaponStateChangedSignal>(OnWeaponStateChanged);
            }

            if (pickUpButton != null)
            {
                pickUpButton.onClick.RemoveListener(OnPickUpClicked);
            }

            if (_inputService != null)
            {
                _inputService.SetMove(Vector2.zero);
                _inputService.SetLook(Vector2.zero);
                _inputService.SetAimPressed(false);
                _inputService.SetFirePressed(false);
            }
        }

        private void Update()
        {
            if (_inputService == null)
            {
                return;
            }

            var move = moveJoystick != null ? moveJoystick.Direction : Vector2.zero;
            var look = lookJoystick != null ? lookJoystick.Direction : Vector2.zero;
            /*move.y = -move.y;
            look.y = -look.y;*/

            _inputService.SetMove(move);
            _inputService.SetLook(look);
        }

        private void OnWeaponStateChanged(WeaponStateChangedSignal signal)
        {
            var canPickUp = signal.HasNearbyWeapon && !signal.HasWeapon;

            if (pickUpButton != null)
            {
                pickUpButton.gameObject.SetActive(canPickUp);
            }

            if (pickupHintRoot != null)
            {
                pickupHintRoot.SetActive(canPickUp);
            }

            if (aimButton != null)
            {
                aimButton.gameObject.SetActive(signal.HasWeapon);
            }

            if (fireButton != null)
            {
                fireButton.gameObject.SetActive(signal.HasWeapon);
            }
        }

        private void OnPickUpClicked()
        {
            _signalBus?.Fire<PickUpWeaponSignal>();
        }

        private void OnAimButtonHoldChanged(bool pressed)
        {
            _inputService?.SetAimPressed(pressed);
        }

        private void OnFireButtonHoldChanged(bool pressed)
        {
            _inputService?.SetFirePressed(pressed);
        }

        private static void BindHoldButton(Button button, System.Action<bool> onHoldChanged)
        {
            if (button == null || onHoldChanged == null)
            {
                return;
            }

            var trigger = button.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = button.gameObject.AddComponent<EventTrigger>();
            }

            if (trigger.triggers == null)
            {
                trigger.triggers = new System.Collections.Generic.List<EventTrigger.Entry>();
            }

            AddTrigger(trigger, EventTriggerType.PointerDown, _ => onHoldChanged(true));
            AddTrigger(trigger, EventTriggerType.PointerUp, _ => onHoldChanged(false));
            AddTrigger(trigger, EventTriggerType.PointerExit, _ => onHoldChanged(false));
            AddTrigger(trigger, EventTriggerType.Cancel, _ => onHoldChanged(false));
        }

        private static void AddTrigger(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> callback)
        {
            var entry = new EventTrigger.Entry { eventID = type };
            entry.callback.AddListener(callback);
            trigger.triggers.Add(entry);
        }
    }
}
