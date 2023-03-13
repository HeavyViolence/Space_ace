using SpaceAce.Levels;
using System;

namespace SpaceAce.Main
{
    public sealed class LevelLoadedEventArgs : EventArgs
    {
        public LevelConfig LevelConfig { get; }

        public LevelLoadedEventArgs(LevelConfig config)
        {
            LevelConfig = config;
        }
    }
}