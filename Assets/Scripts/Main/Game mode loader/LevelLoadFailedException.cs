using System;

namespace SpaceAce.Main
{
    public sealed class LevelLoadFailedException : Exception
    {
        private const string ErrorMessage = "Game mode loader doesn't contain the necessary level config!";

        public (EnemyType type, LevelDifficulty difficulty) MissingLevelConfigIdentity { get; }

        public LevelLoadFailedException(EnemyType type, LevelDifficulty difficulty) : base(ErrorMessage)
        {
            MissingLevelConfigIdentity = (type, difficulty);
        }
    }
}