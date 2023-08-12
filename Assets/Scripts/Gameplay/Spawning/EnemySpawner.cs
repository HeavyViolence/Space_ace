using SpaceAce.Architecture;
using SpaceAce.Gameplay.Inventories;
using SpaceAce.Levels;
using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Spawning
{
    public sealed class EnemySpawner : Spawner, ICombatBeaconUser
    {
        private bool _combatBeaconActive = false;

        public EnemySpawner() { }

        protected override Quaternion GetSpawnedEntityRotation() => Quaternion.Euler(0f, 0f, 180f);

        public override void OnInitialize()
        {
            base.OnInitialize();

            GameServices.Register(this);
        }

        public override void OnClear()
        {
            base.OnClear();

            GameServices.Deregister(this);
        }

        protected override void OnConfigSetup(LevelConfig levelConfig)
        {
            Config = levelConfig.EnemySpawnerConfig;
            Config.EnsureNecessaryObjectPoolsExistence();

            if (_combatBeaconActive) _combatBeaconActive = false;
        }

        public bool Use(CombatBeacon beacon)
        {
            if (beacon is null) throw new ArgumentNullException(nameof(beacon));

            if (_combatBeaconActive)
            {
                return false;
            }
            else
            {
                ToSpawnCount += beacon.AdditionalEnemies;
                AdditionalCountPerWave += beacon.AdditionalWaveLength;
                _combatBeaconActive = true;

                return true;
            }
        }
    }
}