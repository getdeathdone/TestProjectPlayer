using Prototype.Config;
using Prototype.Player;
using Prototype.Signals;
using UnityEngine;
using Zenject;

namespace Prototype.Weapons
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private Transform muzzle;
        [SerializeField] private float range = 120f;
        [SerializeField] private float bulletSpeed = 80f;
        [SerializeField] private float bulletLifetime = 3f;
        [SerializeField] private float bulletRadius = 0.06f;
        [SerializeField] private Vector3 fallbackHoldLocalPosition = new Vector3(0.05f, -0.02f, 0.2f);
        [SerializeField] private Vector3 fallbackHoldLocalEuler = new Vector3(0f, 90f, 0f);
        [SerializeField] private Vector3 fallbackHoldLocalScale = Vector3.one;

        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void Awake()
        {
            if (muzzle != null)
            {
                return;
            }

            var muzzleObject = new GameObject("Muzzle");
            muzzleObject.transform.SetParent(transform);
            muzzleObject.transform.localPosition = Vector3.forward * GameplayConfig.Weapon.DefaultMuzzleForwardOffset;
            muzzleObject.transform.localRotation = Quaternion.identity;
            muzzle = muzzleObject.transform;
        }

        public void AttachTo(Transform container)
        {
            var idle = GetComponent<WeaponIdle>();
            if (idle != null)
            {
                idle.enabled = false;
            }

            transform.SetParent(container);
            var pose = GetComponent<WeaponPickupPose>();
            if (pose != null)
            {
                transform.localPosition = pose.LocalPosition;
                transform.localRotation = Quaternion.Euler(pose.LocalEuler);
                transform.localScale = pose.LocalScale;
            }
            else
            {
                transform.localPosition = fallbackHoldLocalPosition;
                transform.localRotation = Quaternion.Euler(fallbackHoldLocalEuler);
                transform.localScale = fallbackHoldLocalScale;
            }

            var collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
        }

        public void TryFire()
        {
            var camera = UnityEngine.Camera.main;
            if (camera == null)
            {
                return;
            }

            var origin = muzzle != null ? muzzle.position : camera.transform.position;
            var direction = ResolveFireDirection(camera, origin);
            SpawnBullet(origin, direction);
        }

        private Vector3 ResolveFireDirection(UnityEngine.Camera camera, Vector3 origin)
        {
            var screenRay = camera.ViewportPointToRay(new Vector3(GameplayConfig.Weapon.ViewportCenter, GameplayConfig.Weapon.ViewportCenter, 0f));
            var targetPoint = screenRay.origin + screenRay.direction * range;
            if (Physics.Raycast(screenRay, out var hit, range))
            {
                targetPoint = hit.point;
            }

            return (targetPoint - origin).normalized;
        }

        private void SpawnBullet(Vector3 origin, Vector3 direction)
        {
            var bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bullet.name = "Bullet";
            bullet.transform.position = origin + direction * GameplayConfig.Weapon.BulletSpawnForwardOffset;
            bullet.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            bullet.transform.localScale = Vector3.one * bulletRadius * GameplayConfig.Weapon.BulletDiameterMultiplier;

            var renderer = bullet.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;
            }

            var projectile = bullet.AddComponent<BulletProjectile>();
            projectile.Initialize(direction, bulletSpeed, bulletLifetime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_signalBus == null || other.GetComponentInParent<PlayerMovement>() == null)
            {
                return;
            }

            _signalBus.Fire(new WeaponProximitySignal(this, true));
        }

        private void OnTriggerExit(Collider other)
        {
            if (_signalBus == null || other.GetComponentInParent<PlayerMovement>() == null)
            {
                return;
            }

            _signalBus.Fire(new WeaponProximitySignal(this, false));
        }
    }
}
