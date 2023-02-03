using System;
using UnityEngine;

namespace SpaceAce.Main
{
    public enum EnemyType
    {
        Pirates, Aliens, Sweepers
    }

    public enum LevelDifficulty
    {
        Easy, Medium, Hard, Ace
    }

    [CreateAssetMenu(fileName = "Level config", menuName = "Space ace/Configs/Level config")]
    public sealed class LevelConfig : ScriptableObject, IEquatable<LevelConfig>
    {
        [SerializeField] private EnemyType _enemyType = EnemyType.Pirates;
        [SerializeField] private LevelDifficulty _difficulty = LevelDifficulty.Easy;

        public EnemyType EnemyType => _enemyType;
        public LevelDifficulty Difficulty => _difficulty;

        public bool VerifyIdentityMatch(EnemyType type, LevelDifficulty difficulty) => type == EnemyType &&
                                                                                       difficulty == Difficulty;

        public bool VerifyIdentityMatch(LevelConfig config) => config != null &&
                                                               config.EnemyType == EnemyType &&
                                                               config.Difficulty == Difficulty;

        #region IEquatable

        public override bool Equals(object obj) => Equals(obj as LevelConfig);

        public bool Equals(LevelConfig other) => other != null &&
                                                 other.EnemyType.Equals(EnemyType) &&
                                                 other.Difficulty.Equals(Difficulty);

        public static bool operator ==(LevelConfig x, LevelConfig y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return true;
                }

                return false;
            }

            return x.Equals(y);
        }

        public static bool operator !=(LevelConfig x, LevelConfig y) => !(x == y);

        public override int GetHashCode() => _enemyType.GetHashCode() ^ _difficulty.GetHashCode();

        #endregion

        public override string ToString() => $"{Difficulty} {EnemyType.ToString().ToLower()} level";
    }
}