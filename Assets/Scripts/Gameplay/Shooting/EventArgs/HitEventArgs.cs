using SpaceAce.Gameplay.Damageables;
using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    public sealed class HitEventArgs : EventArgs
    {
        public Vector2 HitPosition { get; }
        public IDamageable DamageReceiver { get; }

        public HitEventArgs(Vector2 hitPosition,
                            IDamageable damageReceiver)
        {
            HitPosition = hitPosition;
            DamageReceiver = damageReceiver;
        }
    }
}