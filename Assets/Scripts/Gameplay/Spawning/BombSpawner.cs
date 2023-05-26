using SpaceAce.Architecture;
using SpaceAce.Levels;
using UnityEngine;

namespace SpaceAce.Gameplay.Spawning
{
    public sealed class BombSpawner : Spawner
    {
        public BombSpawner() { }

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

        protected override void OnConfigSetup(LevelConfig config)
        {
            Config = config.BombSpawnerConfig;
            Config.EnsureNecessaryObjectPoolsExistence();
        }

        protected override Quaternion GetSpawnedEntityRotation() => Quaternion.identity;
    }
}