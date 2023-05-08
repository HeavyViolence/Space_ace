using SpaceAce.Architecture;
using SpaceAce.Levels;
using UnityEngine;

namespace SpaceAce.Gameplay.Spawning
{
    public sealed class EnemySpawner : Spawner
    {
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
        }
    }
}