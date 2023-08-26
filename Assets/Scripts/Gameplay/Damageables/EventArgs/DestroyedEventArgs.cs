using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Damageables
{
    public sealed class DestroyedEventArgs : EventArgs
    {
        public Vector2 DeathPosition { get; }
        public float Lifetime { get; }
        public float ExperienceEarned { get; }
        public float ExperienceLost { get; }
        public float ExperienceTotal => ExperienceEarned + ExperienceLost;

        public DestroyedEventArgs(Vector2 deathPosition,
                                  float lifetime,
                                  float experienceEarned,
                                  float experienceLost)
        {
            DeathPosition = deathPosition;
            Lifetime = lifetime;
            ExperienceEarned = experienceEarned;
            ExperienceLost = experienceLost;
        }
    }
}