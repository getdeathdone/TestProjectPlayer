using Cysharp.Threading.Tasks;
using Prototype.Services;
using Prototype.Signals;
using UnityEngine;
using Zenject;
using Prototype.Player;

namespace Prototype.Spawners
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private string playerLabel = "Player";

        private IAddressablesService _addressablesService;
        private SignalBus _signalBus;
        private PlayerBuilder _builder;
        private PlayerService _playerService;

        [Inject]
        public void Construct(
            IAddressablesService addressablesService,
            SignalBus signalBus,
            PlayerBuilder builder,
            PlayerService playerService)
        {
            _addressablesService = addressablesService;
            _signalBus = signalBus;
            _builder = builder;
            _playerService = playerService;
        }

        public async UniTask<GameObject> SpawnPlayerAsync()
        {
            if (_playerService != null && _playerService.PlayerGameObject != null)
            {
                _signalBus.Fire(new PlayerSpawnedSignal(_playerService.PlayerGameObject));
                return _playerService.PlayerGameObject;
            }

            var point = spawnPoint != null ? spawnPoint.position : transform.position;
            var rotation = spawnPoint != null ? spawnPoint.rotation : transform.rotation;

            var instance = await _addressablesService.InstantiateRandom(playerLabel, point, rotation, null);
            if (instance != null)
            {
                _builder.Configure(instance);
                _addressablesService.Inject(instance);
                _signalBus.Fire(new PlayerSpawnedSignal(instance));
            }

            return instance;
        }

        public void SetSpawnPointIfMissing(Transform point)
        {
            if (spawnPoint == null)
            {
                spawnPoint = point;
            }
        }
    }
}
