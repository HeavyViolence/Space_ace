using System;
using System.Collections.Generic;

namespace SpaceAce.Levels
{
    public sealed class BestLevelRunStatistics : IEquatable<BestLevelRunStatistics>,
                                                 IComparable<BestLevelRunStatistics>,
                                                 IComparer<BestLevelRunStatistics>
    {
        public static BestLevelRunStatistics Default => new(0f, 0f, 0f, 0f, (0, 0), 0, 0, 0);

        public float ShootingAccuracy { get; }
        public float PlayerDamagePercentage { get; }
        public float MeteorsCrushedPercentage { get; }
        public float SpaceDebrisCrushedPercentage { get; }
        public int Minutes { get; }
        public int Seconds { get; }
        public int CreditsEarned { get; }
        public int ExperienceEarned { get; }
        public int EnemiesDefeated { get; }

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
            ShootingAccuracy = shootingAccuracy;
            PlayerDamagePercentage = Math.Clamp(playerDamagePercentage, 0f, 1f);
            MeteorsCrushedPercentage = meteorsCrushedPercentage;
            SpaceDebrisCrushedPercentage = spacedebrisCrushedPercentage;
            Minutes = timeSpent.minutes;
            Seconds = timeSpent.seconds;
            CreditsEarned = creditsEarned;
            ExperienceEarned = experienceEarned;
            EnemiesDefeated = enemiesDefeated;
        }

        #region interfaces

        public override bool Equals(object obj) => Equals(obj as BestLevelRunStatistics);

        public bool Equals(BestLevelRunStatistics other) => other is not null && LevelMastery.Equals(other.LevelMastery);

        public override int GetHashCode() => LevelMastery.GetHashCode();

        public int CompareTo(BestLevelRunStatistics other)
        {
            if (LevelMastery > other.LevelMastery) return 1;
            if (LevelMastery < other.LevelMastery) return -1;
            return 0;
        }

        public int Compare(BestLevelRunStatistics x, BestLevelRunStatistics y)
        {
            if (x.LevelMastery > y.LevelMastery) return 1;
            if (x.LevelMastery < y.LevelMastery) return -1;
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

        public static bool operator >(BestLevelRunStatistics x, BestLevelRunStatistics y) => x.LevelMastery > y.LevelMastery;

        public static bool operator <(BestLevelRunStatistics x, BestLevelRunStatistics y) => x.LevelMastery < y.LevelMastery;

        public static bool operator >=(BestLevelRunStatistics x, BestLevelRunStatistics y) => x.LevelMastery >= y.LevelMastery;

        public static bool operator <=(BestLevelRunStatistics x, BestLevelRunStatistics y) => x.LevelMastery <= y.LevelMastery;

        #endregion
    }
}