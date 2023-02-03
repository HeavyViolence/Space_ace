using System;

namespace SpaceAce.Main
{
    public sealed class LoadingStartedEventArgs : EventArgs
    {
        public float Delay { get; }

        public LoadingStartedEventArgs(float delay)
        {
            Delay = delay;
        }
    }
}