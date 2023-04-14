using SpaceAce.Gameplay.Damageables;
using System;

namespace SpaceAce.Gameplay.Players
{
    public sealed class PlayerShipSpawnedEventArgs : EventArgs
    {
        public IDestroyable Destroyable { get; }

        public PlayerShipSpawnedEventArgs(IDestroyable destroyable)
        {
            Destroyable = destroyable;
        }
    }
}