using System;

namespace SpaceAce.Gameplay.Shooting
{
    public interface IGunner
    {
        event EventHandler GunFired, TargetHit;
    }
}