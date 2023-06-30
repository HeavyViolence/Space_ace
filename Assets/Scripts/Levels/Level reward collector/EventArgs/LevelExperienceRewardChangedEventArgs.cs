using System;

namespace SpaceAce.Levels
{
    public sealed class LevelExperienceRewardChangedEventArgs : EventArgs
    {
        public float OldValue { get; }
        public float NewValue { get; }

        public LevelExperienceRewardChangedEventArgs(float oldValue, float newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}