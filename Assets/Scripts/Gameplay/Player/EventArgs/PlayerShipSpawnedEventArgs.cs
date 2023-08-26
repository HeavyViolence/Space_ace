using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Players
{
    public sealed class PlayerShipSpawnedEventArgs : EventArgs
    {
        public GameObject Ship { get; }

        public PlayerShipSpawnedEventArgs(GameObject ship)
        {
            Ship = ship;
        }
    }
}