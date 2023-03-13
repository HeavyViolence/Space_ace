using System;

namespace SpaceAce.Levels
{
    public sealed class LevelDataEventArgs : EventArgs
    {
        public int LevelIndex { get; }

        public LevelDataEventArgs(int levelIndex)
        {
            LevelIndex = levelIndex;
        }
    }
}