using UnityEngine;
using Prototype.Config;

namespace Prototype.Environment
{
    public class EnvironmentBootstrapper : MonoBehaviour
    {
        [SerializeField] private Vector3 groundScale = new Vector3(6f, 1f, 6f);
        [SerializeField] private Transform groundTransform;

        public Transform GroundTransform => groundTransform;

        public void EnsureEnvironment()
        {
            if (groundTransform == null)
            {
                var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
                ground.name = "Ground";
                ground.transform.position = Vector3.zero;
                ground.transform.localScale = groundScale;
                groundTransform = ground.transform;
            }

            for (var i = 0; i < GameplayConfig.Environment.ObstacleCount; i++)
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.name = $"Obstacle_{i + GameplayConfig.Environment.ObstacleNameStartIndex}";
                cube.transform.position = new Vector3(
                    GameplayConfig.Environment.ObstacleStartX + i * GameplayConfig.Environment.ObstacleStepX,
                    GameplayConfig.Environment.ObstacleY,
                    GameplayConfig.Environment.ObstacleStartZ + (i % GameplayConfig.Environment.ObstacleZModulo) * GameplayConfig.Environment.ObstacleZStep);
                cube.transform.localScale = GameplayConfig.Environment.ObstacleScale;
            }
        }
    }
}
