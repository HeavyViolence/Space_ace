namespace SpaceAce.Gameplay.Players
{
    public interface IShootingController
    {
        bool Shoot();
        bool StopShooting();
        bool SwitchToNextWeapons();
        bool SwitchToPreviousWeapons();
    }
}