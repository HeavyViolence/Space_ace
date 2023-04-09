using SpaceAce.Gameplay.Damageables;
using SpaceAce.Gameplay.Movement;
using System;

namespace SpaceAce.Gameplay.Spawning
{
    public sealed class EntitySpawnedEventArgs : EventArgs
    {
        public IEscapable Escapable { get; }
        public IDestroyable Destroyable { get; }

        public EntitySpawnedEventArgs(IEscapable escapable,
                                      IDestroyable destroyable)
        {
            Escapable = escapable;
            Destroyable = destroyable;
        }
    }
}