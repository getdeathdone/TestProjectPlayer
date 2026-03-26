using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Zenject;

namespace Prototype.Services
{
    public class AddressablesService : IAddressablesService, System.IDisposable
    {
        private readonly DiContainer _container;
        private readonly Dictionary<string, IList<IResourceLocation>> _locationsCache = new();
        private readonly Dictionary<string, AsyncOperationHandle> _locationHandles = new();
        private readonly List<AsyncOperationHandle> _instanceHandles = new();

        public AddressablesService(DiContainer container)
        {
            _container = container;
        }

        public async UniTask<GameObject> InstantiateRandom(string label, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (string.IsNullOrWhiteSpace(label))
            {
                return null;
            }

            if (!_locationsCache.TryGetValue(label, out var locations))
            {
                var handle = Addressables.LoadResourceLocationsAsync(label, typeof(GameObject));
                _locationHandles[label] = handle;
                await handle.Task;

                if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null || handle.Result.Count == 0)
                {
                    return null;
                }

                locations = handle.Result;
                _locationsCache[label] = locations;
            }

            if (locations == null || locations.Count == 0)
            {
                return null;
            }

            var randomIndex = Random.Range(0, locations.Count);
            var instanceHandle = Addressables.InstantiateAsync(locations[randomIndex], position, rotation, parent);
            _instanceHandles.Add(instanceHandle);
            var instance = await instanceHandle.Task;
            if (instance == null)
            {
                return null;
            }

            _container.InjectGameObject(instance);
            return instance;
        }

        public void Inject(GameObject instance)
        {
            if (instance == null)
            {
                return;
            }

            _container.InjectGameObject(instance);
        }

        public void Dispose()
        {
            foreach (var handle in _instanceHandles)
            {
                if (handle.IsValid())
                {
                    Addressables.ReleaseInstance(handle);
                }
            }

            foreach (var handle in _locationHandles.Values)
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }

            _instanceHandles.Clear();
            _locationHandles.Clear();
            _locationsCache.Clear();
        }
    }
}
