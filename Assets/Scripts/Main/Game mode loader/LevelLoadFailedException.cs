using System;

namespace SpaceAce.Main
{
    public sealed class LevelLoadFailedException : Exception
    {
        private const string ErrorMessage = "Game mode loader doesn't contain the necessary level config!";

        public int MissingLevelConfig { get; }

        public LevelLoadFailedException(int levelConfig) : base(ErrorMessage)
        {
            MissingLevelConfig = levelConfig;
        }
    }
}