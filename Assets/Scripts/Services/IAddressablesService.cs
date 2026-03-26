using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Prototype.Services
{
    public interface IAddressablesService
    {
        UniTask<GameObject> InstantiateRandom(string label, Vector3 position, Quaternion rotation, Transform parent = null);
        void Inject(GameObject instance);
    }
}
