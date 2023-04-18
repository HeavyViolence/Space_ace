using SpaceAce.Architecture;
using SpaceAce.Auxiliary;
using SpaceAce.Gameplay.Amplifications;
using SpaceAce.Gameplay.Damageables;
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
    public abstract class Spawner: IInitializable
    {
        private const float SpawnPositionWidthIndentFactor = 0.75f;
        private const float SpawnPositionHeightIndentFactor = 1.25f;

        private const float EntityEscapeDelta = 2f;

        private static readonly GameServiceFastAccess<MultiobjectPool> s_multiobjectPool = new();
        private static readonly GameServiceFastAccess<MasterCameraHolder> s_masterCameraHolder = new();

        public event EventHandler SpawnStarted, SpawnPaused, SpawnResumed, SpawnEnded;
        public event EventHandler<EntitySpawnedEventArgs> EntitySpawned;

        private readonly HashSet<(GameObject entity, string anchorname)> _aliveEntities = new();
        private Coroutine _spawningRoutine;

        private float MinHorizontalSpawnPosition => s_masterCameraHolder.Access.ViewportLeftBound * SpawnPositionWidthIndentFactor;
        private float MaxHorizontalSpawnPosition => s_masterCameraHolder.Access.ViewportRightBound * SpawnPositionWidthIndentFactor;
        private float VerticalSpawnPosition => s_masterCameraHolder.Access.ViewportUpperBound * SpawnPositionHeightIndentFactor;

        public SpawnerConfig Config { get; protected set; }
        public int SpawnedAmount { get; protected set; }
        public int AliveAmount => _aliveEntities.Count;
        public int AmountToSpawn { get; protected set; }
        public bool SpawnIsActive => _spawningRoutine != null;

        private void StartSpawn()
        {
            if (SpawnIsActive == false)
            {
                _spawningRoutine = CoroutineRunner.RunRoutine(SpawningRoutine());
            }
        }

        private void StopSpawn()
        {
            if (SpawnIsActive)
            {
                CoroutineRunner.StopRoutine(_spawningRoutine);
                _spawningRoutine = null;
            }
        }

        private IEnumerator SpawningRoutine()
        {
            SpawnStarted?.Invoke(this, EventArgs.Empty);

            while (SpawnedAmount < AmountToSpawn)
            {
                foreach (var (anchorName, spawnDelay) in Config.GetProceduralWave())
                {
                    yield return new WaitForSeconds(spawnDelay);

                    SpawnEntity(anchorName);

                    if (SpawnedAmount == AmountToSpawn)
                    {
                        break;
                    }
                }

                if (Config.HaltUntilClear)
                {
                    SpawnPaused?.Invoke(this, EventArgs.Empty);

                    while (AliveAmount > 0)
                    {
                        yield return null;
                    }

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

            IEscapable escapable;
            IDestroyable destroyable;

            if (entity.TryGetComponent(out IDestroyable d) == true)
            {
                destroyable = d;

                d.Destroyed += (s, e) =>
                {
                    s_multiobjectPool.Access.ReleaseObject(anchorName, entity, () => true);
                    _aliveEntities.Remove((entity, anchorName));
                };
            }
            else
            {
                throw new MissingComponentException($"Spawned entity is missing a mandatory component of type {typeof(IDestroyable)}!");
            }

            if (entity.TryGetComponent(out IEscapable e) == true)
            {
                escapable = e;

                e.BeginWatchForEscape(() => s_masterCameraHolder.Access.InsideViewport(entity.transform.position, EntityEscapeDelta) == false);

                e.Escaped += (s, e) =>
                {
                    s_multiobjectPool.Access.ReleaseObject(anchorName, entity, () => true);
                    _aliveEntities.Remove((entity, anchorName));
                };
            }
            else
            {
                throw new MissingComponentException($"Spawned entity is missing a mandatory component of type {typeof(IEscapable)}!");
            }

            if (AuxMath.Random < Config.AmplificationConfig.AmplificationProbability.RandomValue)
            {
                if (entity.TryGetComponent(out Amplifier amplifier) == true)
                {
                    amplifier.Amplify(Config.AmplificationConfig.AmplificationFactor);
                    Config.AmplificationConfig.AmplifiedEntitySpawnAudio.PlayRandomAudioClip(Vector2.zero);
                }
                else
                {
                    throw new MissingComponentException($"Spawned entity is missing a mandatory component of type {typeof(Amplifier)}!");
                }
            }

            SpawnedAmount++;
            EntitySpawned?.Invoke(this, new(escapable, destroyable));
        }

        private Vector3 GetSpawnedEntityPosition()
        {
            float x = UnityEngine.Random.Range(MinHorizontalSpawnPosition, MaxHorizontalSpawnPosition);
            float y = VerticalSpawnPosition;
            float z = 0f;

            return new Vector3(x, y, z);
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
                loader.MainMenuLoaded += MainMenuLoadedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(GameModeLoader));
            }

            if (GameServices.TryGetService(out LevelCompleter completer) == true)
            {
                completer.LevelConcluded += LevelConcludedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(LevelCompleter));
            }
        }

        public virtual void OnUnsubscribe()
        {
            if (GameServices.TryGetService(out GameModeLoader loader) == true)
            {
                loader.LevelLoaded -= LevelLoadedEventHandler;
                loader.MainMenuLoaded -= MainMenuLoadedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(GameModeLoader));
            }

            if (GameServices.TryGetService(out LevelCompleter completer) == true)
            {
                completer.LevelConcluded -= LevelConcludedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(LevelCompleter));
            }
        }

        public virtual void OnClear()
        {

        }

        #endregion

        #region event handlers

        private void LevelLoadedEventHandler(object sender, LevelLoadedEventArgs e)
        {
            OnConfigSetup(e.LevelConfig);

            SpawnedAmount = 0;
            AmountToSpawn = Config.AmountToSpawn.RandomValue;

            StartSpawn();
        }

        private void LevelConcludedEventHandler(object sender, EventArgs e)
        {
            StopSpawn();

            Config = null;
        }

        private void MainMenuLoadedEventHandler(object sender, EventArgs e)
        {
            foreach (var (entity, anchorName) in _aliveEntities)
            {
                s_multiobjectPool.Access.ReleaseObject(anchorName, entity, () => true);
            }

            _aliveEntities.Clear();
        }

        protected abstract void OnConfigSetup(LevelConfig config);

        #endregion
    }
}