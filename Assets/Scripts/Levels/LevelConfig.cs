using SpaceAce.Gameplay.Spawning;
using SpaceAce.Main.ObjectPooling;
using System;
using UnityEngine;

namespace SpaceAce.Levels
{
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

        [SerializeField] private int _crystalsReward = MinCrystalsReward;
        [SerializeField] private int _experienceReward = MinExperienceReward;

        [SerializeField] private bool _bossEnabled = false;
        [SerializeField] ObjectPoolEntry _boss;

        [SerializeField] private SpawnerConfig _enemySpawnerConfig;
        [SerializeField] private SpawnerConfig _meteorSpawnerConfig;
        [SerializeField] private SpawnerConfig _spaceDebrisSpawnerConfig;

        public int LevelIndex => _levelIndex;

        public int CrystalsReward => _crystalsReward;
        public int ExperienceReward => _experienceReward;

        public bool BossEnabled => _bossEnabled;
        public ObjectPoolEntry Boss => _boss;

        public SpawnerConfig EnemySpawnerConfig => _enemySpawnerConfig;
        public SpawnerConfig MeteorSpawnerConfig => _meteorSpawnerConfig;
        public SpawnerConfig SpaceDebrisSpawnerConfig => _spaceDebrisSpawnerConfig;

        #region IEquatable

        public override bool Equals(object obj) => Equals(obj as LevelConfig);

        public bool Equals(LevelConfig other) => other != null && other.LevelIndex.Equals(LevelIndex);

        public override int GetHashCode() => LevelIndex.GetHashCode();

        #endregion
    }
}