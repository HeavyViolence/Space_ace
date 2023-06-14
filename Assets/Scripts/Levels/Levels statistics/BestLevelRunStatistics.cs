using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Levels
{
    [Serializable]
    public sealed class BestLevelRunStatistics : IEquatable<BestLevelRunStatistics>,
                                                 IComparable<BestLevelRunStatistics>,
                                                 IComparer<BestLevelRunStatistics>
    {
        public static BestLevelRunStatistics Default => new(0f, 0f, 0f, 0f, (0, 0), 0, 0, 0);

        [SerializeField] private float _shootingAccuracy;
        [SerializeField] private float _playerDamagePercentage;
        [SerializeField] private float _meteorsCrushedPercentage;
        [SerializeField] private float _spaceDebrisCrushedPercentage;
        [SerializeField] private int _minutes;
        [SerializeField] private int _seconds;
        [SerializeField] private int _credistEarned;
        [SerializeField] private int _experienceEarned;
        [SerializeField] private int _enemiesDefeated;

        public float ShootingAccuracy => _shootingAccuracy;
        public float PlayerDamagePercentage => +_playerDamagePercentage;
        public float MeteorsCrushedPercentage => _meteorsCrushedPercentage;
        public float SpaceDebrisCrushedPercentage => _spaceDebrisCrushedPercentage;
        public (int minutes, int seconds) TimeSpent => (_minutes, _seconds);
        public int CreditsEarned => _credistEarned;
        public int ExperienceEarned => _experienceEarned;
        public int EnemiesDefeated => _enemiesDefeated;
        public float LevelMastery => ShootingAccuracy *
                                     MeteorsCrushedPercentage *
                                     SpaceDebrisCrushedPercentage /
                                     (1f + PlayerDamagePercentage);

        public BestLevelRunStatistics(float shootingAccuracy,
                                      float playerDamagePercentage,
                                      float meteorsCrushedPercentage,
                                      float spacedebrisCrushedPercentage,
                                      (int minutes, int seconds) timeSpent,
                                      int creditsEarned,
                                      int experienceEarned,
                                      int enemiesDefeated)
        {
            _shootingAccuracy = shootingAccuracy;
            _playerDamagePercentage = Math.Clamp(playerDamagePercentage, 0f, 1f);
            _meteorsCrushedPercentage = meteorsCrushedPercentage;
            _spaceDebrisCrushedPercentage = spacedebrisCrushedPercentage;
            _minutes = timeSpent.minutes;
            _seconds = timeSpent.seconds;
            _credistEarned = creditsEarned;
            _experienceEarned = experienceEarned;
            _enemiesDefeated = enemiesDefeated;
        }

        #region interfaces

        public override bool Equals(object obj) => Equals(obj as BestLevelRunStatistics);

        public bool Equals(BestLevelRunStatistics other) => other is not null && LevelMastery.Equals(other.LevelMastery);

        public override int GetHashCode() => LevelMastery.GetHashCode();

        public int CompareTo(BestLevelRunStatistics other)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other), $"Attempted to pass an empty {nameof(BestLevelRunStatistics)}!");
            }

            if (LevelMastery > other.LevelMastery)
            {
                return 1;
            }

            if (LevelMastery < other.LevelMastery)
            {
                return -1;
            }

            return 0;
        }

        public int Compare(BestLevelRunStatistics x, BestLevelRunStatistics y)
        {
            if (x is null)
            {
                throw new ArgumentNullException(nameof(x), $"Attempted to pass an empty {nameof(BestLevelRunStatistics)}!");
            }

            if (y is null)
            {
                throw new ArgumentNullException(nameof(y), $"Attempted to pass an empty {nameof(BestLevelRunStatistics)}!");
            }

            if (x.LevelMastery > y.LevelMastery)
            {
                return 1;
            }

            if (x.LevelMastery < y.LevelMastery)
            {
                return -1;
            }

            return 0;
        }

        #endregion

        #region operators overloading

        public static bool operator ==(BestLevelRunStatistics x, BestLevelRunStatistics y)
        {
            if (x is null)
            {
                if (y is null)
                {
                    return true;
                }

                return false;
            }

            return x.Equals(y);
        }

        public static bool operator !=(BestLevelRunStatistics x, BestLevelRunStatistics y) => !(x == y);

        public static bool operator >(BestLevelRunStatistics x, BestLevelRunStatistics y)
        {
            if (x is null)
            {
                throw new ArgumentNullException(nameof(x), $"Attempted to pass an empty {nameof(BestLevelRunStatistics)}!");
            }

            if (y is null)
            {
                throw new ArgumentNullException(nameof(y), $"Attempted to pass an empty {nameof(BestLevelRunStatistics)}!");
            }

            return x.LevelMastery > y.LevelMastery;
        }

        public static bool operator <(BestLevelRunStatistics x, BestLevelRunStatistics y)
        {
            if (x is null)
            {
                throw new ArgumentNullException(nameof(x), $"Attempted to pass an empty {nameof(BestLevelRunStatistics)}!");
            }

            if (y is null)
            {
                throw new ArgumentNullException(nameof(y), $"Attempted to pass an empty {nameof(BestLevelRunStatistics)}!");
            }

            return x.LevelMastery < y.LevelMastery;
        }

        public static bool operator >=(BestLevelRunStatistics x, BestLevelRunStatistics y)
        {
            if (x is null)
            {
                throw new ArgumentNullException(nameof(x), $"Attempted to pass an empty {nameof(BestLevelRunStatistics)}!");
            }

            if (y is null)
            {
                throw new ArgumentNullException(nameof(y), $"Attempted to pass an empty {nameof(BestLevelRunStatistics)}!");
            }

            return x.LevelMastery >= y.LevelMastery;
        }

        public static bool operator <=(BestLevelRunStatistics x, BestLevelRunStatistics y)
        {
            if (x is null)
            {
                throw new ArgumentNullException(nameof(x), $"Attempted to pass an empty {nameof(BestLevelRunStatistics)}!");
            }

            if (y is null)
            {
                throw new ArgumentNullException(nameof(y), $"Attempted to pass an empty {nameof(BestLevelRunStatistics)}!");
            }

            return x.LevelMastery <= y.LevelMastery;
        }

        #endregion
    }
}