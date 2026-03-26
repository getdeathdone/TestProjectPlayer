using UnityEngine;

namespace Prototype.Config
{
    public static class AnimationHashConstant
    {
        public static class StateName
        {
            public const string Idle = "HumanM@Idle01";
            public const string Move = "HumanM@Walk01_Forward";
            public const string RifleIdle = "HumanM@WeaponHold_Rifle01";
            public const string RifleMove = "HumanM@Walk01_Forward";
            public const string RifleEquip = "HumanM@Rifle_Aim01";
            public const string RifleAimIdle = "HumanM@Rifle_Aim01";
            public const string RifleAimMove = "HumanM@Rifle_Aim01";
        }

        public static class StateHash
        {
            public static readonly int Idle = Animator.StringToHash(StateName.Idle);
            public static readonly int Move = Animator.StringToHash(StateName.Move);
            public static readonly int RifleIdle = Animator.StringToHash(StateName.RifleIdle);
            public static readonly int RifleMove = Animator.StringToHash(StateName.RifleMove);
            public static readonly int RifleEquip = Animator.StringToHash(StateName.RifleEquip);
            public static readonly int RifleAimIdle = Animator.StringToHash(StateName.RifleAimIdle);
            public static readonly int RifleAimMove = Animator.StringToHash(StateName.RifleAimMove);
        }

        public static string GetStateName(int stateHash)
        {
            if (stateHash == StateHash.Idle) return StateName.Idle;
            if (stateHash == StateHash.Move) return StateName.Move;
            if (stateHash == StateHash.RifleIdle) return StateName.RifleIdle;
            if (stateHash == StateHash.RifleMove) return StateName.RifleMove;
            if (stateHash == StateHash.RifleEquip) return StateName.RifleEquip;
            if (stateHash == StateHash.RifleAimIdle) return StateName.RifleAimIdle;
            if (stateHash == StateHash.RifleAimMove) return StateName.RifleAimMove;
            return stateHash.ToString();
        }
    }
}
