using System;

namespace SpaceAce.Levels
{
    public sealed class LevelCreditRewardChangeEventArgs : EventArgs
    {
        public int OldValue { get; }
        public int NewValue { get; }

        public LevelCreditRewardChangeEventArgs(int oldValue, int newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}