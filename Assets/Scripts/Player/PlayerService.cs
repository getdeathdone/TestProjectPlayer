using Prototype.Signals;
using UnityEngine;
using Zenject;

namespace Prototype.Player
{
    public  class PlayerService : IInitializable, System.IDisposable
    {
        private readonly SignalBus _signalBus;

        public Transform PlayerTransform { get; private set; }
        public Transform WeaponContainer { get; private set; }
        public GameObject PlayerGameObject => PlayerTransform != null ? PlayerTransform.gameObject : null;

        public PlayerService(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<PlayerSpawnedSignal>(OnPlayerSpawned);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<PlayerSpawnedSignal>(OnPlayerSpawned);
        }

        private void OnPlayerSpawned(PlayerSpawnedSignal signal)
        {
            if (signal.Player == null)
            {
                return;
            }

            PlayerTransform = signal.Player.transform;
            WeaponContainer = FindWeaponContainer(signal.Player.transform);
        }
        
        private static Transform FindWeaponContainer(Transform root)
        {
            if (root == null)
            {
                return null;
            }

            foreach (var child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child != null && child.name == "WeaponContainer")
                {
                    return child;
                }
            }

            return null;
        }
    }
}
