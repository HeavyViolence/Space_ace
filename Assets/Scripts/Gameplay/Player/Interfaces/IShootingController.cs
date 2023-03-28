namespace SpaceAce.Gameplay.Players
{
    public interface IShootingController
    {
        bool Shoot();
        void StopShooting();
        bool SwitchToNextWeapons();
        bool SwitchToPreviousWeapons();
    }
}