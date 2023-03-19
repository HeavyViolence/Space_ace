namespace SpaceAce.Gameplay.Shooting
{
    public interface IGun
    {
        int ID { get; }
        bool Fire();
        bool StopFire();
    }
}