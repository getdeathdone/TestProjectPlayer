using UnityEngine;

namespace Prototype.Weapons
{
    public class WeaponIdle : MonoBehaviour
    {
        [SerializeField] private float rotateSpeed = 60f;
        [SerializeField] private float hoverAmplitude = 0.15f;
        [SerializeField] private float hoverSpeed = 2f;

        private Vector3 _startLocalPosition;

        private void Awake()
        {
            _startLocalPosition = transform.localPosition;
        }

        private void Update()
        {
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
            var offset = Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
            transform.localPosition = _startLocalPosition + Vector3.up * offset;
        }
    }
}
