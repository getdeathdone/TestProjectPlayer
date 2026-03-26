using Prototype.AI;
using UnityEngine;

namespace Prototype.Weapons
{
    [RequireComponent(typeof(SphereCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public class BulletProjectile : MonoBehaviour
    {
        private float _speed;
        private float _lifetime;
        private float _timer;
        private bool _initialized;

        public void Initialize(Vector3 direction, float speed, float lifetime)
        {
            _speed = speed;
            _lifetime = lifetime;
            _initialized = true;

            var rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.velocity = direction.normalized * _speed;
        }

        private void Awake()
        {
            var collider = GetComponent<SphereCollider>();
            collider.isTrigger = true;
        }

        private void Update()
        {
            if (!_initialized)
            {
                return;
            }

            _timer += Time.deltaTime;
            if (_timer >= _lifetime)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<Prototype.Player.PlayerMovement>() != null)
            {
                return;
            }

            var hitPoint = other.ClosestPoint(transform.position);
            Debug.Log($"GameObject: {other.gameObject.name}, HitPoint: {hitPoint}");

            var enemy = other.GetComponentInParent<EnemyWander>();
            if (enemy != null)
            {
                Destroy(enemy.gameObject);
                Destroy(gameObject);
                return;
            }

            if (!other.isTrigger)
            {
                Destroy(gameObject);
            }
        }
    }
}
