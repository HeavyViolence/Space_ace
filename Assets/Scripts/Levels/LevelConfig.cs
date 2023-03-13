using SpaceAce.Main.ObjectPooling;
using System;
using UnityEngine;

namespace SpaceAce.Levels
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
        public const int MinLevelIndex = 1;
        public const int MaxLevelIndex = 15;

        public const int MinCrystalsReward = 100;
        public const int MaxCrystalsReward = 10_000;

        public const int MinExperienceReward = 1_000;
        public const int MaxExperienceReward = 1_000_000;

        [SerializeField] private int _levelIndex = MinLevelIndex;

        [SerializeField] private EnemyType _enemyType = EnemyType.Pirates;
        [SerializeField] private LevelDifficulty _difficulty = LevelDifficulty.Easy;

        [SerializeField] private int _crystalsReward = MinCrystalsReward;
        [SerializeField] private int _experienceReward = MinExperienceReward;

        [SerializeField] private bool _bossEnabled = false;
        [SerializeField] ObjectPoolEntry _boss;

        public int LevelIndex => _levelIndex;

        public EnemyType EnemyType => _enemyType;
        public LevelDifficulty Difficulty => _difficulty;

        public int CrystalsReward => _crystalsReward;
        public int ExperienceReward => _experienceReward;

        public bool BossEnabled => _bossEnabled;
        public ObjectPoolEntry Boss => _boss;

        #region IEquatable

        public override bool Equals(object obj) => Equals(obj as LevelConfig);

        public bool Equals(LevelConfig other) => other != null && other.LevelIndex.Equals(LevelIndex);

        public override int GetHashCode() => LevelIndex.GetHashCode();

        #endregion
    }
}