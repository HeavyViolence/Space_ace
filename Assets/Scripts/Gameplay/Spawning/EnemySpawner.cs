using SpaceAce.Architecture;
using SpaceAce.Main;
using UnityEngine;

namespace SpaceAce.Gameplay.Spawning
{
    public sealed class EnemySpawner : Spawner
    {
        public EnemySpawner() { }

        protected override Quaternion GetSpawnedEntityRotation() => Quaternion.Euler(0f, 0f, 180f);

        public override void OnInitialize()
        {
            GameServices.Register(this);
        }

        public override void OnClear()
        {
            GameServices.Deregister(this);
        }

        protected override void LevelLoadedEventHandler(object sender, LevelLoadedEventArgs e)
        {
            Config = e.LevelConfig.EnemySpawnerConfig;
            Config.EnsureNecessaryObjectPoolsExistence();

            AmountToSpawn = Config.AmountToSpawn.RandomValue;

            base.LevelLoadedEventHandler(sender, e);
        }
    }
}