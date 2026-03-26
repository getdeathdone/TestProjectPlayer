using Prototype.Weapons;

namespace Prototype.Signals
{
    public readonly struct WeaponProximitySignal
    {
        public readonly Weapon Weapon;
        public readonly bool IsNear;

        public WeaponProximitySignal(Weapon weapon, bool isNear)
        {
            Weapon = weapon;
            IsNear = isNear;
        }
    }
}
