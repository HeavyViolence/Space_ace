using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SpaceAce.Levels
{
    [DataContract]
    public sealed class BestLevelRunStatistics : IEquatable<BestLevelRunStatistics>,
                                                 IComparable<BestLevelRunStatistics>,
                                                 IComparer<BestLevelRunStatistics>
    {
        public static BestLevelRunStatistics Default => new BestLevelRunStatistics(0f, 0f, 0f, 0f, (0, 0), 0, 0, 0);

        [DataMember]
        public float ShootingAccuracy { get; private set; }

        [DataMember]
        public float PlayerDamagePercentage { get; private set; }

        [DataMember]
        public float MeteorsCrushedPercentage { get; private set; }

        [DataMember]
        public float SpaceDebrisCrushedPercentage { get; private set; }

        [DataMember]
        public (int minutes, int seconds) TimeSpent { get; private set; }

        [DataMember]
        public int CrystalsEarned { get; private set; }

        [DataMember]
        public int ExperienceEarned { get; private set; }

        [DataMember]
        public int EnemiesDefeated { get; private set; }

        public float LevelMastery => ShootingAccuracy *
                                     MeteorsCrushedPercentage *
                                     SpaceDebrisCrushedPercentage /
                                     (1f + PlayerDamagePercentage);

        public BestLevelRunStatistics(float shootingAccuracy,
                                      float playerDamagePercentage,
                                      float meteorsCrushedPercentage,
                                      float spacedebrisCrushedPercentage,
                                      (int minutes, int seconds) timeSpent,
                                      int crystalsEarned,
                                      int experienceEarned,
                                      int enemiesDefeated)
        {
            ShootingAccuracy = shootingAccuracy;
            PlayerDamagePercentage = Math.Clamp(playerDamagePercentage, 0f, 1f);
            MeteorsCrushedPercentage = meteorsCrushedPercentage;
            SpaceDebrisCrushedPercentage = spacedebrisCrushedPercentage;
            TimeSpent = timeSpent;
            CrystalsEarned = crystalsEarned;
            ExperienceEarned = experienceEarned;
            EnemiesDefeated = enemiesDefeated;
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