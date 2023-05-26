using System;

namespace SpaceAce.Gameplay.Damageables
{
    public interface IDestroyable
    {
        event EventHandler BeforeDestroyed;
        event EventHandler<DestroyedEventArgs> Destroyed;
    }
}