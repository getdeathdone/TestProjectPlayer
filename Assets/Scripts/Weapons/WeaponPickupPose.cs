using UnityEngine;

namespace Prototype.Weapons
{
    public class WeaponPickupPose : MonoBehaviour
    {
        [SerializeField] private Vector3 localPosition = new Vector3(0.05f, -0.02f, 0.2f);
        [SerializeField] private Vector3 localEuler = new Vector3(0f, 90f, 0f);
        [SerializeField] private Vector3 localScale = Vector3.one;

        public Vector3 LocalPosition => localPosition;
        public Vector3 LocalEuler => localEuler;
        public Vector3 LocalScale => localScale;
    }
}
