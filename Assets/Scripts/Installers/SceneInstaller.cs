using Prototype.Input;
using Prototype.Player;
using Prototype.Signals;
using Prototype.Services;
using Prototype.Camera;
using Prototype.Environment;
using Prototype.Spawners;
using Prototype.UI;
using Prototype.Weapons;
using UnityEngine;
using Zenject;

namespace Prototype.Installers
{
    public class SceneInstaller : MonoInstaller
    {
        [Header("Scene References")]
        [SerializeField] private Transform weaponSpawnPoint;
        [SerializeField] private EnvironmentBootstrapper environmentBootstrapper;
        [SerializeField] private PlayerSpawner playerSpawner;
        [SerializeField] private EnemySpawner enemySpawner;
        [SerializeField] private DecorationSpawner decorationSpawner;
        [SerializeField] private CameraController cameraController;

        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            DeclareSignals();

            Container.Bind<IInputService>().To<InputService>().AsSingle();
            Container.Bind<IAddressablesService>().To<AddressablesService>().AsSingle();

            Container.BindInstance(weaponSpawnPoint).IfNotBound();

            if (environmentBootstrapper != null)
            {
                Container.BindInstance(environmentBootstrapper).IfNotBound();
            }

            if (playerSpawner != null)
            {
                Container.BindInstance(playerSpawner).IfNotBound();
            }

            if (enemySpawner != null)
            {
                Container.BindInstance(enemySpawner).IfNotBound();
            }

            if (decorationSpawner != null)
            {
                Container.BindInstance(decorationSpawner).IfNotBound();
            }

            if (cameraController != null)
            {
                Container.BindInstance(cameraController).IfNotBound();
            }

            Container.Bind<PlayerBuilder>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerService>().AsSingle();
            Container.BindInterfacesAndSelfTo<WeaponService>().AsSingle();
            Container.BindInterfacesAndSelfTo<WeaponProvider>().AsSingle();
        }

        private void DeclareSignals()
        {
            Container.DeclareSignal<PickUpWeaponSignal>();
            Container.DeclareSignal<WeaponProximitySignal>();
            Container.DeclareSignal<WeaponSpawnedSignal>();
            Container.DeclareSignal<WeaponStateChangedSignal>();
            Container.DeclareSignal<PlayerSpawnedSignal>();
        }
    }
}
