using System;
using Prototype.Config;
using Prototype.Services;
using Prototype.Signals;
using UnityEngine;
using Zenject;

namespace Prototype.Weapons
{
    public class WeaponProvider : IInitializable, IDisposable
    {
        private readonly Transform _spawnPoint;
        private readonly IAddressablesService _addressablesService;
        private readonly SignalBus _signalBus;
        private const string WeaponsLabel = "Weapons";

        public WeaponProvider(
            Transform spawnPoint,
            IAddressablesService addressablesService,
            SignalBus signalBus)
        {
            _spawnPoint = spawnPoint;
            _addressablesService = addressablesService;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            if (_spawnPoint == null)
            {
                return;
            }

            SpawnWeapon().Forget();
        }

        private async Cysharp.Threading.Tasks.UniTaskVoid SpawnWeapon()
        {
            var instance = await _addressablesService.InstantiateRandom(
                WeaponsLabel,
                _spawnPoint.position + Vector3.up * GameplayConfig.Spawner.WeaponSpawnHeightOffset,
                _spawnPoint.rotation,
                null);
            if (instance == null)
            {
                return;
            }

            var weapon = EnsureWeaponSetup(instance);
            _addressablesService.Inject(instance);
            if (weapon != null)
            {
                _signalBus.Fire(new WeaponSpawnedSignal(weapon));
            }
        }

        public void Dispose()
        {
        }

        private Weapon EnsureWeaponSetup(GameObject instance)
        {
            var collider = instance.GetComponent<Collider>();
            if (collider == null)
            {
                var sphere = instance.AddComponent<SphereCollider>();
                sphere.radius = GameplayConfig.Spawner.WeaponTriggerRadius;
                collider = sphere;
            }

            collider.isTrigger = true;
            collider.enabled = true;

            if (!instance.TryGetComponent<Rigidbody>(out var rigidbody))
            {
                rigidbody = instance.AddComponent<Rigidbody>();
            }

            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;

            if (!instance.TryGetComponent<WeaponIdle>(out _))
            {
                instance.AddComponent<WeaponIdle>();
            }

            if (!instance.TryGetComponent<Weapon>(out var weapon))
            {
                weapon = instance.AddComponent<Weapon>();
            }

            return weapon;
        }
    }
}
