using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SpaceAce.Levels
{
    public sealed class BestLevelRunStatistics : IEquatable<BestLevelRunStatistics>,
                                                 IComparable<BestLevelRunStatistics>,
                                                 IComparer<BestLevelRunStatistics>
    {
        public static BestLevelRunStatistics Default => new(0, 0, 0, 0, 0, 0, 0, 0f, 0f, 0f, 0f, 0, 0f, 0f, (0, 0));

        public int EnemiesKilled { get; }

        public int SpaceDebrisDestroyed { get; }
        public int SpaceDebrisMissed { get; }

        [JsonIgnore]
        public float SpaceDebrisMastery => SpaceDebrisDestroyed == 0 ? 0f : SpaceDebrisDestroyed / (SpaceDebrisDestroyed + SpaceDebrisMissed);

        public int MeteorsDestroyed { get; }
        public int MeteorsMissed { get; }

        [JsonIgnore]
        public float MeteorsMastery => MeteorsDestroyed == 0 ? 0f : MeteorsDestroyed / (MeteorsDestroyed + MeteorsMissed);

        public int ShotsFired { get; }
        public int TargetHits { get; }

        [JsonIgnore]
        public int ShotsMissed => ShotsFired - TargetHits;

        [JsonIgnore]
        public float ShootingMastery => ShotsFired == 0 ? 0f : (ShotsFired + ShotsMissed) / (float)ShotsFired;

        public float DamageReceived { get; }
        public float DamageTaken { get; }

        [JsonIgnore]
        public float DamageAverted => DamageReceived - DamageTaken;

        [JsonIgnore]
        public float DamageTakenMastery => DamageReceived == 0f ? 0f : DamageTaken / DamageReceived;

        public float DamageDelivered { get; }
        public float DamageDealt { get; }

        [JsonIgnore]
        public float DamageLost => DamageDelivered - DamageDealt;

        [JsonIgnore]
        public float DamageDealtMastery => DamageDelivered == 0f ? 0f : DamageDealt / DamageDelivered;

        public int CreditsEarned { get; }

        public float ExperienceEarned { get; }
        public float ExperienceLost { get; }

        [JsonIgnore]
        public float ExperienceMastery => ExperienceEarned == 0f ? 0f : ExperienceEarned / (ExperienceEarned + ExperienceLost);

        public (int minutes, int seconds) TimeSpent { get; }

        [JsonIgnore]
        public float LevelMastery => SpaceDebrisMastery *
                                     MeteorsMastery *
                                     ShootingMastery *
                                     DamageTakenMastery *
                                     DamageDealtMastery *
                                     ExperienceMastery;

        public BestLevelRunStatistics(int enemiesKilled,
                                      int spaceDebrisDestroyed,
                                      int spaceDebrisMissed,
                                      int meteorsDestroyed,
                                      int meteorsMissed,
                                      int shotsFired,
                                      int targetHits,
                                      float damageReceived,
                                      float damageTaken,
                                      float damageDelivered,
                                      float damageDealt,
                                      int creditsEarned,
                                      float experienceEarned,
                                      float experienceLost,
                                      (int minutes, int seconds) timeSpent)
        {
            EnemiesKilled = enemiesKilled;
            SpaceDebrisDestroyed = spaceDebrisDestroyed;
            SpaceDebrisMissed = spaceDebrisMissed;
            MeteorsDestroyed = meteorsDestroyed;
            MeteorsMissed = meteorsMissed;
            ShotsFired = shotsFired;
            TargetHits = targetHits;
            DamageReceived = damageReceived;
            DamageTaken = damageTaken;
            DamageDelivered = damageDelivered;
            DamageDealt = damageDealt;
            CreditsEarned = creditsEarned;
            ExperienceEarned = experienceEarned;
            ExperienceLost = experienceLost;
            TimeSpent = timeSpent;
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
                if (y is null) return true;

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