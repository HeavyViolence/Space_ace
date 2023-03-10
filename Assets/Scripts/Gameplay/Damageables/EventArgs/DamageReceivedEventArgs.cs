using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Damageables
{
    public sealed class DamageReceivedEventArgs : EventArgs
    {
        public float DamageReceived { get; }
        public float DamageDealt { get; }
        public float DamageEfficiency => DamageDealt / DamageReceived;
        public Vector2 HitPosition { get; }

        public DamageReceivedEventArgs(float damageReceived,
                                       float damageDealt,
                                       Vector2 hitPosition)
        {
            DamageReceived = damageReceived;
            DamageDealt = damageDealt;
            HitPosition = hitPosition;
        }
    }
}