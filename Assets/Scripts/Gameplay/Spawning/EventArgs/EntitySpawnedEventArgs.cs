using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Spawning
{
    public sealed class EntitySpawnedEventArgs : EventArgs
    {
        public GameObject Entity { get; }

        public EntitySpawnedEventArgs(GameObject entity)
        {
            Entity = entity;
        }
    }
}