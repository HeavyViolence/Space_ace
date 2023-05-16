namespace SpaceAce.Gameplay.Shooting
{
    public interface IGun
    {
        int GroupID { get; }
        bool ReadyToFire { get; }
        float MaxDamagePerSecond { get; }

        bool Fire();
        bool StopFire();
    }
}