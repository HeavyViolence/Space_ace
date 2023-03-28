namespace SpaceAce.Gameplay.Shooting
{
    public interface IGun
    {
        int GunGroupID { get; }
        bool ReadyToFire { get; }
        bool Fire();
        bool StopFire();
    }
}