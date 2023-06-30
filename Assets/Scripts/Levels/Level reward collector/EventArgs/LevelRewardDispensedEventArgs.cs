using System;

namespace SpaceAce.Levels
{
    public sealed class LevelRewardDispensedEventArgs : EventArgs
    {
        public int Credits { get; }
        public float Experience { get; }

        public LevelRewardDispensedEventArgs(int credits, float experience)
        {
            Credits = credits;
            Experience = experience;
        }
    }
}