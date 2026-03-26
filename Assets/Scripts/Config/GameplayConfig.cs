using UnityEngine;

namespace Prototype.Config
{
    public static class GameplayConfig
    {
        public static class CharacterControllerDefaults
        {
            public const float Height = 2f;
            public const float Radius = 0.4f;
            public const float CenterY = 1f;
            public const float GroundedVerticalVelocity = -2f;
        }

        public static class Weapon
        {
            public const float SpawnNearbyDistance = 3f;
            public const float AutoPickupDistance = 2.5f;
            public const float FireCooldown = 0.1f;
            public const float DefaultMuzzleForwardOffset = 0.6f;
            public const float BulletSpawnForwardOffset = 0.15f;
            public const float BulletDiameterMultiplier = 2f;
            public const float ViewportCenter = 0.5f;
        }

        public static class Spawner
        {
            public const float SpawnHeightOffset = 5f;
            public const float GroundRaycastDistance = 100f;
            public const float WeaponSpawnHeightOffset = 1f;
            public const float WeaponTriggerRadius = 1.2f;
            public static readonly Vector3 WeaponContainerFallbackLocalPosition = new Vector3(0.3f, 1.2f, 0.5f);
        }

        public static class Environment
        {
            public const int ObstacleCount = 6;
            public const int ObstacleNameStartIndex = 1;
            public const float ObstacleStartX = -8f;
            public const float ObstacleStepX = 3f;
            public const float ObstacleY = 0.5f;
            public const float ObstacleStartZ = 6f;
            public const int ObstacleZModulo = 2;
            public const float ObstacleZStep = 3f;
            public const float DecorationProjectionHeightOffset = 5f;
            public static readonly Vector3 ObstacleScale = new Vector3(1.5f, 1.5f, 1.5f);
        }

        public static class Camera
        {
            public const int FireMouseButton = 0;
            public const int AimMouseButton = 1;
            public const float LookInputSqrThreshold = 0.0001f;
        }

        public static class Movement
        {
            public const float DirectionSqrThreshold = 0.001f;
            public const float MaxInputMagnitude = 1f;
            public const float FullCircleDegrees = 360f;
        }
    }
}
