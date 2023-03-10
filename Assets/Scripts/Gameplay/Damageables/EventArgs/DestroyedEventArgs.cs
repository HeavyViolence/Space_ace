using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Damageables
{
    public sealed class DestroyedEventArgs : EventArgs
    {
        public Vector2 DeathPosition { get; }
        public float Lifetime { get; }
        public float EarnedExperience { get; }
        public float LostExperience { get; }
        public float TotalExperience { get; }

        public DestroyedEventArgs(Vector2 deathPosition,
                                  float lifetime,
                                  float earnedExperience,
                                  float lostExperience,
                                  float totalExperience)
        {
            DeathPosition = deathPosition;
            Lifetime = lifetime;
            EarnedExperience = earnedExperience;
            LostExperience = lostExperience;
            TotalExperience = totalExperience;
        }
    }
}