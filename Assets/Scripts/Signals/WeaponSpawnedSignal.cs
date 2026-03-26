using Prototype.Weapons;

namespace Prototype.Signals
{
    public readonly struct WeaponSpawnedSignal
    {
        public readonly Weapon Weapon;

        public WeaponSpawnedSignal(Weapon weapon)
        {
            Weapon = weapon;
        }
    }
}
