using Prototype.Config;
using UnityEngine;

namespace Prototype.Player
{
    public class PlayerBuilder
    {
        public void Configure(GameObject instance)
        {
            if (instance == null)
            {
                return;
            }

            EnsureWeaponContainer(instance.transform);
            EnsureCharacterController(instance);
            EnsurePlayerMovement(instance);
            EnsureAnimatorBridge(instance);
        }

        private static void EnsureCharacterController(GameObject instance)
        {
            if (instance.TryGetComponent<CharacterController>(out _))
            {
                return;
            }

            var controller = instance.AddComponent<CharacterController>();
            controller.height = GameplayConfig.CharacterControllerDefaults.Height;
            controller.radius = GameplayConfig.CharacterControllerDefaults.Radius;
            controller.center = new Vector3(0f, GameplayConfig.CharacterControllerDefaults.CenterY, 0f);
        }

        private static void EnsurePlayerMovement(GameObject instance)
        {
            if (!instance.TryGetComponent<PlayerMovement>(out _))
            {
                instance.AddComponent<PlayerMovement>();
            }
        }

        private static void EnsureAnimatorBridge(GameObject instance)
        {
            var animator = instance.GetComponentInChildren<Animator>(true);
            if (animator == null)
            {
                return;
            }

            if (!animator.TryGetComponent<WeaponAnimatorBridge>(out var bridge))
            {
                bridge = animator.gameObject.AddComponent<WeaponAnimatorBridge>();
            }

            bridge.enabled = true;
        }

        private static void EnsureWeaponContainer(Transform root)
        {
            if (root.Find("WeaponContainer") != null)
            {
                return;
            }

            var animator = root.GetComponentInChildren<Animator>(true);
            var hand = FindHand(root, animator);
            var targetParent = hand != null ? hand : root;

            var container = new GameObject("WeaponContainer").transform;
            container.SetParent(targetParent);
            container.localPosition = hand != null ? Vector3.zero : GameplayConfig.Spawner.WeaponContainerFallbackLocalPosition;
            container.localRotation = Quaternion.identity;
        }

        private static Transform FindHand(Transform root, Animator animator)
        {
            if (animator != null && animator.isHuman)
            {
                var humanHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
                if (humanHand != null)
                {
                    return humanHand;
                }
            }

            var names = new[]
            {
                "RightHand",
                "Hand_R",
                "RightHandMiddle1",
                "mixamorig:RightHand",
                "r_hand",
                "R_Hand",
                "hand.r",
                "hand_r",
                "r_hand_end"
            };

            foreach (var name in names)
            {
                var found = root.Find(name);
                if (found != null)
                {
                    return found;
                }
            }

            var skinned = root.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (var renderer in skinned)
            {
                var bones = renderer.bones;
                if (bones == null)
                {
                    continue;
                }

                foreach (var bone in bones)
                {
                    if (bone == null)
                    {
                        continue;
                    }

                    var lower = bone.name.ToLowerInvariant();
                    if (lower.Contains("hand") && (lower.Contains("right") || lower.Contains("r_") || lower.EndsWith("_r") || lower.EndsWith(".r")))
                    {
                        return bone;
                    }
                }
            }

            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                var lower = child.name.ToLowerInvariant();
                if (lower.Contains("hand") && (lower.Contains("right") || lower.Contains("r_") || lower.EndsWith("_r")))
                {
                    return child;
                }
            }

            return null;
        }
    }
}
