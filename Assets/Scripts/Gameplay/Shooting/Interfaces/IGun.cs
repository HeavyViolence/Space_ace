using System;

namespace SpaceAce.Gameplay.Shooting
{
    public interface IGun
    {
        public event EventHandler ShotFired, Hit;

        int GroupID { get; }
        bool ReadyToFire { get; }
        float MaxDamagePerSecond { get; }

        bool Fire();
        bool StopFire();
    }
}