using System;

namespace SpaceAce.Gameplay.Damageables
{
    public interface IDestroyable
    {
        event EventHandler<DestroyedEventArgs> Destroyed;
    }
}