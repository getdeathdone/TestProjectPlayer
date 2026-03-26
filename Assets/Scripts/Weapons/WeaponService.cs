using System;
using Prototype.Config;
using Prototype.Input;
using Prototype.Player;
using Prototype.Signals;
using UnityEngine;
using Zenject;

namespace Prototype.Weapons
{
    public class WeaponService : IInitializable, ITickable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly IInputService _inputService;
        private readonly PlayerService _playerService;

        private Weapon _nearbyWeapon;
        private Weapon _equippedWeapon;
        private Weapon _spawnedWeapon;
        private float _nextFireTime;

        public WeaponService(SignalBus signalBus, IInputService inputService, PlayerService playerService)
        {
            _signalBus = signalBus;
            _inputService = inputService;
            _playerService = playerService;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<WeaponProximitySignal>(OnWeaponProximity);
            _signalBus.Subscribe<PickUpWeaponSignal>(OnPickUp);
            _signalBus.Subscribe<WeaponSpawnedSignal>(OnWeaponSpawned);
            EmitState();
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<WeaponProximitySignal>(OnWeaponProximity);
            _signalBus.Unsubscribe<PickUpWeaponSignal>(OnPickUp);
            _signalBus.Unsubscribe<WeaponSpawnedSignal>(OnWeaponSpawned);
        }

        private void OnWeaponSpawned(WeaponSpawnedSignal signal)
        {
            _nearbyWeapon = null;
            _spawnedWeapon = signal.Weapon;
            EmitState();

            var player = _playerService.PlayerTransform;
            if (player != null)
            {
                var distance = Vector3.Distance(player.position, signal.Weapon.transform.position);
                if (distance <= GameplayConfig.Weapon.SpawnNearbyDistance)
                {
                    _nearbyWeapon = signal.Weapon;
                    EmitState();
                }
            }
        }

        public void Tick()
        {
            if (_equippedWeapon != null)
            {
                HandleMouseFireInput();
                return;
            }

            if (_nearbyWeapon != null && UnityEngine.Input.GetKeyDown(KeyCode.E))
            {
                OnPickUp();
                return;
            }

            if (_spawnedWeapon == null)
            {
                return;
            }

            var player = _playerService.PlayerTransform;
            if (player == null)
            {
                return;
            }

            var distance = Vector3.Distance(player.position, _spawnedWeapon.transform.position);
            if (distance <= GameplayConfig.Weapon.AutoPickupDistance)
            {
                if (_nearbyWeapon != _spawnedWeapon)
                {
                    _nearbyWeapon = _spawnedWeapon;
                    EmitState();
                }
            }
            else if (_nearbyWeapon == _spawnedWeapon)
            {
                _nearbyWeapon = null;
                EmitState();
            }
        }

        private void HandleMouseFireInput()
        {
            if (_equippedWeapon == null)
            {
                return;
            }

            if (!IsAiming())
            {
                return;
            }

            if (!IsFiringPressed())
            {
                return;
            }

            if (Time.time < _nextFireTime)
            {
                return;
            }

            _nextFireTime = Time.time + GameplayConfig.Weapon.FireCooldown;
            OnFire();
        }

        private void OnWeaponProximity(WeaponProximitySignal signal)
        {
            if (_equippedWeapon != null)
            {
                return;
            }

            if (signal.IsNear)
            {
                _nearbyWeapon = signal.Weapon;
            }
            else if (_nearbyWeapon == signal.Weapon)
            {
                _nearbyWeapon = null;
            }

            EmitState();
        }

        private void OnPickUp()
        {
            if (_equippedWeapon != null || _nearbyWeapon == null)
            {
                return;
            }

            var container = _playerService.WeaponContainer;
            if (container == null)
            {
                return;
            }

            _equippedWeapon = _nearbyWeapon;
            _nearbyWeapon = null;
            _equippedWeapon.AttachTo(container);
            EmitState();
        }

        private void OnFire()
        {
            if (_equippedWeapon == null)
            {
                return;
            }

            if (!IsAiming())
            {
                return;
            }

            _equippedWeapon.TryFire();
        }

        private bool IsAiming()
        {
            return (IsMouseInputEnabled() && UnityEngine.Input.GetMouseButton(GameplayConfig.Camera.AimMouseButton)) ||
                   (_inputService != null && _inputService.AimPressed);
        }

        private bool IsFiringPressed()
        {
            return (IsMouseInputEnabled() && UnityEngine.Input.GetMouseButton(GameplayConfig.Camera.FireMouseButton)) ||
                   (_inputService != null && _inputService.FirePressed);
        }

        private static bool IsMouseInputEnabled()
        {
            return !Application.isMobilePlatform;
        }

        private void EmitState()
        {
            _signalBus.Fire(new WeaponStateChangedSignal(_equippedWeapon != null, _nearbyWeapon != null));
        }
    }
}
