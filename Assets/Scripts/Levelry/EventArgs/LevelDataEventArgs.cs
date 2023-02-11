using SpaceAce.Main;
using System;

namespace SpaceAce.Levelry
{
    public sealed class LevelDataEventArgs : EventArgs
    {
        public EnemyType EnemyType { get; }
        public LevelDifficulty Difficulty { get; }

        public LevelDataEventArgs(EnemyType enemyType,
                                  LevelDifficulty difficulty)
        {
            EnemyType = enemyType;
            Difficulty = difficulty;
        }
    }
}