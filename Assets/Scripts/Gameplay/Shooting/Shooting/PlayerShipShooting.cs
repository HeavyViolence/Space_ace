using SpaceAce.Gameplay.Players;

namespace SpaceAce.Gameplay.Shooting
{
    public sealed class PlayerShipShooting : Shooting, IShootingController
    {
        public bool Shoot() => NextActiveGun.Fire();

        public void StopShooting() => StopActiveWeaponGroupShooting();

        public bool SwitchToNextWeapons() => ActivateNextWeaponGroup(true);

        public bool SwitchToPreviousWeapons() => ActivatePreviousWeaponGroup(true);
    }
}