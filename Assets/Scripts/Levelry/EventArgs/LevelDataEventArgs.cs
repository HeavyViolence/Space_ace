using System;

namespace SpaceAce.Levelry
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