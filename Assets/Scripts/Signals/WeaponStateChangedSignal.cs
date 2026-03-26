namespace Prototype.Signals
{
    public readonly struct WeaponStateChangedSignal
    {
        public readonly bool HasWeapon;
        public readonly bool HasNearbyWeapon;

        public WeaponStateChangedSignal(bool hasWeapon, bool hasNearbyWeapon)
        {
            HasWeapon = hasWeapon;
            HasNearbyWeapon = hasNearbyWeapon;
        }
    }
}
