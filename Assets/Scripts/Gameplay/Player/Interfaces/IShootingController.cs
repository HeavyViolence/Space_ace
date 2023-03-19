namespace SpaceAce.Gameplay.Players
{
    public interface IShootingController
    {
        int AvailableWeaponGroups { get; }

        void SwitchToPreviousWeaponGroup();
        void SwitchToNextWeaponGroup();
        void Shoot();
    }
}