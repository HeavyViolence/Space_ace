using SpaceAce.Architecture;
using SpaceAce.Auxiliary;
using SpaceAce.Gameplay.Amplifications;
using SpaceAce.Gameplay.Damageables;
using SpaceAce.Gameplay.Inventories;
using SpaceAce.Gameplay.Movement;
using SpaceAce.Levels;
using SpaceAce.Main;
using SpaceAce.Main.ObjectPooling;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Gameplay.Spawning
{
    public abstract class Spawner : IGameService
    {
        private const float SpawnPositionWidthIndentFactor = 0.75f;
        private const float SpawnPositionHeightIndentFactor = 1.25f;

        private static readonly GameServiceFastAccess<MultiobjectPool> s_multiobjectPool = new();
        private static readonly GameServiceFastAccess<MasterCameraHolder> s_masterCameraHolder = new();
        protected static readonly GameServiceFastAccess<GamePauser> GamePauser = new();
        protected static readonly GameServiceFastAccess<GameModeLoader> GameModeLoader = new();

        public event EventHandler SpawnStarted, SpawnPaused, SpawnResumed, SpawnEnded;
        public event EventHandler<EntitySpawnedEventArgs> EntitySpawned;

        private readonly HashSet<(GameObject entity, string anchorname)> _aliveEntities = new();
        private Coroutine _spawningRoutine;

        private float MinHorizontalSpawnPosition => s_masterCameraHolder.Access.ViewportLeftBound * SpawnPositionWidthIndentFactor;
        private float MaxHorizontalSpawnPosition => s_masterCameraHolder.Access.ViewportRightBound * SpawnPositionWidthIndentFactor;
        private float VerticalSpawnPosition => s_masterCameraHolder.Access.ViewportUpperBound * SpawnPositionHeightIndentFactor;

        public SpawnerConfig Config { get; protected set; }
        public int SpawnedCount { get; protected set; }
        public int ALiveCount => _aliveEntities.Count;
        public int ToSpawnCount { get; protected set; }
        public int DestroyedCount { get; private set; }
        public int AdditionalCountPerWave { get; protected set; }
        public float SpawnDelayFactor { get; protected set; }
        public bool SpawnIsActive => _spawningRoutine != null;

        private void StartSpawn()
        {
            if (SpawnIsActive == true) return;

            _spawningRoutine = CoroutineRunner.RunRoutine(SpawningRoutine());
        }

        private void StopSpawn()
        {
            if (SpawnIsActive == false) return;

            CoroutineRunner.StopRoutine(_spawningRoutine);
            _spawningRoutine = null;
        }

        private IEnumerator SpawningRoutine()
        {
            SpawnStarted?.Invoke(this, EventArgs.Empty);

            while (SpawnedCount < ToSpawnCount)
            {
                foreach (var (anchorName, spawnDelay) in Config.GetProceduralWave(AdditionalCountPerWave))
                {
                    yield return CoroutineRunner.RunRoutine(SpawnDelayer(spawnDelay * SpawnDelayFactor));

                    SpawnEntity(anchorName);

                    if (SpawnedCount == ToSpawnCount) break;
                }

                if (Config.HaltUntilClear)
                {
                    SpawnPaused?.Invoke(this, EventArgs.Empty);

                    while (ALiveCount > 0) yield return null;

                    SpawnResumed?.Invoke(this, EventArgs.Empty);
                }
            }

            SpawnEnded?.Invoke(this, EventArgs.Empty);
            _spawningRoutine = null;
        }

        private void SpawnEntity(string anchorName)
        {
            Vector3 spawnPosition = GetSpawnedEntityPosition();

            var entity = s_multiobjectPool.Access.GetObject(anchorName);
            entity.transform.SetPositionAndRotation(spawnPosition, GetSpawnedEntityRotation());

            _aliveEntities.Add((entity, anchorName));

            if (entity.TryGetComponent(out IDestroyable d) == true)
            {
                d.Destroyed += (s, e) =>
                {
                    s_multiobjectPool.Access.ReleaseObject(anchorName, entity, () => true);
                    _aliveEntities.Remove((entity, anchorName));
                    DestroyedCount++;
                };
            }
            else
            {
                throw new MissingComponentException(typeof(IDestroyable).ToString());
            }

            if (entity.TryGetComponent(out IEscapable e) == true)
            {
                e.StartWatchingForEscape(() => s_masterCameraHolder.Access.InsideViewport(entity.transform.position, Config.EscapeDelta) == false);

                e.Escaped += (s, e) =>
                {
                    s_multiobjectPool.Access.ReleaseObject(anchorName, entity, () => true);
                    _aliveEntities.Remove((entity, anchorName));
                };
            }
            else
            {
                throw new MissingComponentException(typeof(IEscapable).ToString());
            }

            if (Config.AmplificationEnabled && AuxMath.Random < Config.AmplificationConfig.AmplificationProbability.RandomValue)
            {
                if (entity.TryGetComponent(out Amplifier amplifier) == true)
                {
                    amplifier.Amplify(Config.AmplificationConfig.AmplificationFactor);
                    Config.AmplificationConfig.AmplifiedEntitySpawnAudio.PlayRandomAudioClip(Vector2.zero);
                }
                else
                {
                    throw new MissingComponentException(typeof(Amplifier).ToString());
                }
            }

            SpawnedCount++;
            EntitySpawned?.Invoke(this, new(entity));
        }

        private Vector3 GetSpawnedEntityPosition()
        {
            float x = UnityEngine.Random.Range(MinHorizontalSpawnPosition, MaxHorizontalSpawnPosition);
            float y = VerticalSpawnPosition;
            float z = 0f;

            return new Vector3(x, y, z);
        }

        private IEnumerator SpawnDelayer(float delay)
        {
            float timer = 0f;

            while (timer < delay)
            {
                timer += Time.deltaTime;

                yield return null;
                while (GamePauser.Access.Paused == true) yield return null;
            }
        }

        protected virtual Quaternion GetSpawnedEntityRotation() => Quaternion.identity;

        #region interfaces

        public virtual void OnInitialize()
        {

        }

        public virtual void OnSubscribe()
        {
            if (GameServices.TryGetService(out GameModeLoader loader) == true)
            {
                loader.LevelLoaded += LevelLoadedEventHandler;

                loader.MainMenuLoadingStarted += MainMenuLoadingStartedEventHandler;
                loader.MainMenuLoaded += MainMenuLoadedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(GameModeLoader));
            }

            if (GameServices.TryGetService(out LevelCompleter completer) == true) completer.LevelConcluded += LevelConcludedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(LevelCompleter));

            if (GameServices.TryGetService(out SpecialEffectsMediator mediator) == true) mediator.Register(this);
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(SpecialEffectsMediator));
        }

        public virtual void OnUnsubscribe()
        {
            if (GameServices.TryGetService(out GameModeLoader loader) == true)
            {
                loader.LevelLoaded -= LevelLoadedEventHandler;

                loader.MainMenuLoadingStarted -= MainMenuLoadingStartedEventHandler;
                loader.MainMenuLoaded -= MainMenuLoadedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(GameModeLoader));
            }

            if (GameServices.TryGetService(out LevelCompleter completer) == true) completer.LevelConcluded -= LevelConcludedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(LevelCompleter));

            if (GameServices.TryGetService(out SpecialEffectsMediator mediator) == true) mediator.Deregister(this);
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(SpecialEffectsMediator));
        }

        public virtual void OnClear()
        {

        }

        #endregion

        #region event handlers

        private void LevelLoadedEventHandler(object sender, LevelLoadedEventArgs e)
        {
            OnConfigSetup(e.LevelConfig);

            SpawnedCount = 0;
            ToSpawnCount = Config.AmountToSpawn.RandomValue;
            DestroyedCount = 0;
            AdditionalCountPerWave = 0;
            SpawnDelayFactor = 1f;

            StartSpawn();
        }

        private void LevelConcludedEventHandler(object sender, EventArgs e)
        {
            StopSpawn();
            Config = null;
        }

        private void MainMenuLoadingStartedEventHandler(object sender, LoadingStartedEventArgs e)
        {
            StopSpawn();
            Config = null;
        }

        private void MainMenuLoadedEventHandler(object sender, EventArgs e)
        {
            foreach (var (entity, anchorName) in _aliveEntities) s_multiobjectPool.Access.ReleaseObject(anchorName, entity, () => true);
            _aliveEntities.Clear();
        }

        protected abstract void OnConfigSetup(LevelConfig config);

        #endregion
    }
}