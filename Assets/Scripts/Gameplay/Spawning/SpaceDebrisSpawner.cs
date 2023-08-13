using SpaceAce.Architecture;
using SpaceAce.Auxiliary;
using SpaceAce.Gameplay.Inventories;
using SpaceAce.Levels;
using SpaceAce.Main;
using System;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Gameplay.Spawning
{
    public sealed class SpaceDebrisSpawner : Spawner, ISpaceDebrisRouterUser
    {
        private Coroutine _spaceDebrisRouter = null;

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

        public bool Use(SpaceDebrisRouter router)
        {
            if (router is null) throw new ArgumentNullException(nameof(router));

            if (_spaceDebrisRouter == null)
            {
                _spaceDebrisRouter = CoroutineRunner.RunRoutine(ApplySpaceDebrisRouter(router));
                return true;
            }

            return false;
        }

        private IEnumerator ApplySpaceDebrisRouter(SpaceDebrisRouter router)
        {
            SpawnDelayFactor = 1f / router.SpaceDebrisSpawnSpeedup;
            float timer = 0f;

            while (timer < router.Duration)
            {
                timer += Time.deltaTime;

                if (GameModeLoader.Access.GameState != GameState.Level)
                {
                    SpawnDelayFactor = 1f;
                    _spaceDebrisRouter = null;

                    yield break;
                }

                yield return null;
                while (GamePauser.Access.Paused == true) yield return null;
            }

            SpawnDelayFactor = 1f;
            _spaceDebrisRouter = null;
        }
    }
}