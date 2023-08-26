using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Damageables
{
    public sealed class DamageReceivedEventArgs : EventArgs
    {
        public float DamageReceived { get; }
        public float DamageTaken { get; }
        public float DamageLost => DamageReceived - DamageTaken;
        public float DamageEfficiency => DamageTaken / DamageReceived;
        public Vector2 HitPosition { get; }

        public DamageReceivedEventArgs(float damageReceived,
                                       float damageDealt,
                                       Vector2 hitPosition)
        {
            DamageReceived = damageReceived;
            DamageTaken = damageDealt;
            HitPosition = hitPosition;
        }
    }
}