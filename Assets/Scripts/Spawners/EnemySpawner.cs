using Cysharp.Threading.Tasks;
using Prototype.AI;
using Prototype.Config;
using Prototype.Services;
using UnityEngine;
using Zenject;

namespace Prototype.Spawners
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private int spawnCount = 5;
        [SerializeField] private float spawnRadius = 12f;
        [SerializeField] private LayerMask groundMask = ~0;
        [SerializeField] private string enemiesLabel = "Enemies";

        private IAddressablesService _addressablesService;

        [Inject]
        public void Construct(IAddressablesService addressablesService)
        {
            _addressablesService = addressablesService;
        }

        public async UniTask SpawnEnemiesAsync()
        {
            if (_addressablesService == null)
            {
                return;
            }

            for (var i = 0; i < spawnCount; i++)
            {
                var position = GetRandomPoint();
                if (!TryProjectToGround(position, out var hit))
                {
                    continue;
                }

                var instance = await _addressablesService.InstantiateRandom(enemiesLabel, hit.point, Quaternion.identity, null);
                if (instance == null)
                {
                    continue;
                }

                if (!instance.TryGetComponent<CharacterController>(out var controller))
                {
                    controller = instance.AddComponent<CharacterController>();
                    controller.height = GameplayConfig.CharacterControllerDefaults.Height;
                    controller.radius = GameplayConfig.CharacterControllerDefaults.Radius;
                    controller.center = new Vector3(0f, GameplayConfig.CharacterControllerDefaults.CenterY, 0f);
                }

                if (!instance.TryGetComponent<EnemyWander>(out _))
                {
                    instance.AddComponent<EnemyWander>();
                }
            }
        }

        private Vector3 GetRandomPoint()
        {
            var offset = Random.insideUnitSphere * spawnRadius;
            offset.y = 0f;
            return transform.position + offset + Vector3.up * GameplayConfig.Spawner.SpawnHeightOffset;
        }

        private bool TryProjectToGround(Vector3 position, out RaycastHit hit)
        {
            return Physics.Raycast(position, Vector3.down, out hit, GameplayConfig.Spawner.GroundRaycastDistance, groundMask);
        }
    }
}
