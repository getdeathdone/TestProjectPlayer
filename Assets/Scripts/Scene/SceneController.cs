using Cysharp.Threading.Tasks;
using Prototype.Camera;
using Prototype.Environment;
using Prototype.Spawners;
using UnityEngine;
using Zenject;

namespace Prototype.Scene
{
    public class SceneController : MonoBehaviour
    {
        [Header("Scene Bootstrap")]
        private EnvironmentBootstrapper environmentBootstrapper;
        private PlayerSpawner playerSpawner;
        private CameraController cameraController;
        private DecorationSpawner decorationSpawner;
        private EnemySpawner enemySpawner;
        private Transform autoSpawnPoint;

        private DiContainer _container;

        [Inject]
        public void Construct(
            DiContainer container,
            [InjectOptional] Transform injectedAutoSpawnPoint = null,
            [InjectOptional] EnvironmentBootstrapper injectedEnvironmentBootstrapper = null,
            [InjectOptional] PlayerSpawner injectedPlayerSpawner = null,
            [InjectOptional] EnemySpawner injectedEnemySpawner = null,
            [InjectOptional] DecorationSpawner injectedDecorationSpawner = null,
            [InjectOptional] CameraController injectedCameraController = null)
        {
            _container = container;
            autoSpawnPoint = injectedAutoSpawnPoint;
            environmentBootstrapper = injectedEnvironmentBootstrapper;
            playerSpawner = injectedPlayerSpawner;
            enemySpawner = injectedEnemySpawner;
            decorationSpawner = injectedDecorationSpawner;
            cameraController = injectedCameraController;
        }

        private void Awake()
        {
            ResolveDependencies();
        }

        private async void Start()
        {
            await BootstrapAsync();
        }

        private async UniTask BootstrapAsync()
        {
            environmentBootstrapper?.EnsureEnvironment();
            if (decorationSpawner != null && environmentBootstrapper != null)
            {
                decorationSpawner.SetTargetPlaneIfMissing(environmentBootstrapper.GroundTransform);
            }

            if (playerSpawner != null)
            {
                await playerSpawner.SpawnPlayerAsync();
            }

            await UniTask.Yield();
            cameraController?.SyncYawWithTarget();

            if (decorationSpawner != null)
            {
                await decorationSpawner.SpawnDecorationsAsync();
            }

            if (enemySpawner != null)
            {
                await enemySpawner.SpawnEnemiesAsync();
            }
        }

        private void ResolveDependencies()
        {
            environmentBootstrapper = ResolveOrCreateComponent(
                environmentBootstrapper,
                "EnvironmentBootstrapper");

            playerSpawner = ResolveOrCreateComponent(
                playerSpawner,
                "PlayerSpawner");

            enemySpawner = ResolveOrCreateComponent(
                enemySpawner,
                "EnemySpawner");

            decorationSpawner = ResolveOrCreateComponent(
                decorationSpawner,
                "DecorationSpawner");

            cameraController = ResolveOrCreateCameraController(cameraController);

            if (playerSpawner != null)
            {
                playerSpawner.SetSpawnPointIfMissing(autoSpawnPoint != null ? autoSpawnPoint : transform);
            }
        }

        private T ResolveOrCreateComponent<T>(T existing, string objectName) where T : Component
        {
            if (existing != null)
            {
                return existing;
            }

            var createdObject = new GameObject(objectName);
            createdObject.transform.SetParent(transform);
            var created = createdObject.AddComponent<T>();
            _container?.Inject(created);
            return created;
        }

        private CameraController ResolveOrCreateCameraController(CameraController existing)
        {
            if (existing != null)
            {
                return existing;
            }

            var camera = UnityEngine.Camera.main;
            if (camera == null)
            {
                var cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                camera = cameraObject.AddComponent<UnityEngine.Camera>();
            }

            var controller = camera.gameObject.GetComponent<CameraController>();
            if (controller == null)
            {
                controller = camera.gameObject.AddComponent<CameraController>();
            }

            _container?.Inject(controller);
            return controller;
        }
    }
}
