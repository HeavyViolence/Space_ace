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
    public sealed class MeteorSpawner : Spawner, IMeteorRouterUser
    {
        private Coroutine _meteorRouter = null;

        public MeteorSpawner() { }

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
            Config = config.MeteorSpawnerConfig;
            Config.EnsureNecessaryObjectPoolsExistence();
        }

        public bool Use(MeteorRouter router)
        {
            if (router is null) throw new ArgumentNullException(nameof(router));

            if (_meteorRouter == null)
            {
                _meteorRouter = CoroutineRunner.RunRoutine(ApplyMeteorRouterRouter(router));
                return true;
            }

            return false;
        }

        private IEnumerator ApplyMeteorRouterRouter(MeteorRouter router)
        {
            SpawnDelayFactor = 1f / router.MeteorSpawnSpeedup;
            float timer = 0f;

            while (timer < router.Duration)
            {
                timer += Time.deltaTime;

                if (GameModeLoader.Access.GameState != GameState.Level)
                {
                    SpawnDelayFactor = 1f;
                    _meteorRouter = null;

                    yield break;
                }

                yield return null;
                while (GamePauser.Access.Paused == true) yield return null;
            }

            SpawnDelayFactor = 1f;
            _meteorRouter = null;
        }
    }
}