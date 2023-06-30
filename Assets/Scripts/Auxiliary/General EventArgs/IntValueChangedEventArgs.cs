using System;

namespace SpaceAce.Auxiliary
{
    public sealed class IntValueChangedEventArgs : EventArgs
    {
        public int OldValue { get; }
        public int NewValue { get; }

        public IntValueChangedEventArgs(int oldValue, int newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}