using SpaceAce.Architecture;
using SpaceAce.Auxiliary;
using SpaceAce.Levels;
using UnityEngine;

namespace SpaceAce.Gameplay.Spawning
{
    public sealed class SpaceDebrisSpawner : Spawner
    {
        public SpaceDebrisSpawner() { }

        protected override Quaternion GetSpawnedEntityRotation() => Quaternion.Euler(0f, 0f, 360f * AuxMath.Random);

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
            Config = config.SpaceDebrisSpawnerConfig;
            Config.EnsureNecessaryObjectPoolsExistence();
        }
    }
}