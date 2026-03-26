using Cysharp.Threading.Tasks;
using Prototype.Config;
using Prototype.Services;
using UnityEngine;
using Zenject;

namespace Prototype.Environment
{
    public class DecorationSpawner : MonoBehaviour
    {
        [SerializeField] private Transform targetPlane;
        [SerializeField] private int spawnCount = 40;
        [SerializeField] private float edgePadding = 1f;
        [SerializeField] private Vector2 scaleRange = new Vector2(0.8f, 1.3f);
        [SerializeField] private Vector2 yRotationRange = new Vector2(0f, 360f);
        [SerializeField] private LayerMask groundMask = ~0;
        [SerializeField] private bool alignToNormal = false;
        [SerializeField] private string decorationLabel = "Decoration";

        private IAddressablesService _addressablesService;

        [Inject]
        public void Construct(IAddressablesService addressablesService)
        {
            _addressablesService = addressablesService;
        }

        [ContextMenu("Spawn Decorations")]
        public void SpawnDecorations()
        {
            SpawnDecorationsAsync().Forget();
        }

        public void SetTargetPlaneIfMissing(Transform plane)
        {
            if (targetPlane == null)
            {
                targetPlane = plane;
            }
        }

        public async UniTask SpawnDecorationsAsync()
        {
            if (targetPlane == null || _addressablesService == null)
            {
                return;
            }

            var bounds = GetPlaneBounds();
            if (bounds.size == Vector3.zero)
            {
                return;
            }

            for (var i = 0; i < spawnCount; i++)
            {
                var position = GetRandomPoint(bounds);
                if (!TryProjectToGround(position, out var hit))
                {
                    continue;
                }

                var rotation = Quaternion.Euler(0f, Random.Range(yRotationRange.x, yRotationRange.y), 0f);
                if (alignToNormal)
                {
                    rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * rotation;
                }

                var decoration = await _addressablesService.InstantiateRandom(
                    decorationLabel,
                    hit.point,
                    rotation,
                    transform);
                if (decoration == null)
                {
                    continue;
                }

                var scale = Random.Range(scaleRange.x, scaleRange.y);
                decoration.transform.localScale *= scale;
                EnsureCollider(decoration);
            }
        }

        private static void EnsureCollider(GameObject decoration)
        {
            if (decoration == null)
            {
                return;
            }

            if (decoration.GetComponentInChildren<Collider>(true) != null)
            {
                return;
            }

            var meshFilter = decoration.GetComponentInChildren<MeshFilter>(true);
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                var meshCollider = meshFilter.gameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = meshFilter.sharedMesh;
                meshCollider.convex = false;
                meshCollider.isTrigger = false;
                return;
            }

            var renderer = decoration.GetComponentInChildren<Renderer>(true);
            if (renderer != null)
            {
                var boxCollider = renderer.gameObject.AddComponent<BoxCollider>();
                boxCollider.isTrigger = false;
                return;
            }

            var fallback = decoration.AddComponent<BoxCollider>();
            fallback.isTrigger = false;
        }

        private Bounds GetPlaneBounds()
        {
            if (targetPlane.TryGetComponent<Collider>(out var collider))
            {
                return collider.bounds;
            }

            if (targetPlane.TryGetComponent<Renderer>(out var renderer))
            {
                return renderer.bounds;
            }

            return new Bounds();
        }

        private Vector3 GetRandomPoint(Bounds bounds)
        {
            var min = bounds.min;
            var max = bounds.max;

            min.x += edgePadding;
            min.z += edgePadding;
            max.x -= edgePadding;
            max.z -= edgePadding;

            var x = Random.Range(min.x, max.x);
            var z = Random.Range(min.z, max.z);
            return new Vector3(x, max.y + GameplayConfig.Environment.DecorationProjectionHeightOffset, z);
        }

        private bool TryProjectToGround(Vector3 position, out RaycastHit hit)
        {
            return Physics.Raycast(position, Vector3.down, out hit, GameplayConfig.Spawner.GroundRaycastDistance, groundMask);
        }
    }
}
