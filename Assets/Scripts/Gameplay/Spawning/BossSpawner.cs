using SpaceAce.Architecture;
using SpaceAce.Gameplay.Damageables;
using SpaceAce.Gameplay.Movement;
using SpaceAce.Main;
using SpaceAce.Main.Audio;
using SpaceAce.Main.ObjectPooling;
using System;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Gameplay.Spawning
{
    public sealed class BossSpawner : IGameService
    {
        private const float BossSpawnDelay = 5f;
        private const float BossSpawnPositionHeightIndentFactor = 1.25f;
        private const float BossEscapeDelta = 2f;

        private static readonly GameServiceFastAccess<MasterCameraHolder> s_masterCameraHolder = new();
        private static readonly GameServiceFastAccess<MultiobjectPool> s_multiobjectPool = new();

        public event EventHandler<EntitySpawnedEventArgs> BossSpawned;

        private Vector2 BossSpawnPosition => new Vector2(0f, s_masterCameraHolder.Access.ViewportUpperBound) * BossSpawnPositionHeightIndentFactor;
        private Quaternion BossSpawnRotation => Quaternion.Euler(new Vector3(0f, 0f, 0f));

        private AudioCollection _bossSpawnAlarm;

        private ObjectPoolEntry _bossToSpawn;
        private GameObject _spawnedBoss;
        private IDestroyable _bossDestroyable;
        private IEscapable _bossEscapable;

        private Coroutine _bossSpawningRoutine;

        public bool Active { get; private set; } = false;
        public bool BossIsAlive { get; private set; } = false;

        public BossSpawner(AudioCollection bossSpawnAlarm)
        {
            if (bossSpawnAlarm == null)
            {
                throw new ArgumentNullException(nameof(bossSpawnAlarm), $"Attempted to pass an empty boss spawn alarm {nameof(AudioCollection)}!");
            }

            _bossSpawnAlarm = bossSpawnAlarm;
        }

        #region interfaces

        public void OnInitialize()
        {
            GameServices.Register(this);
        }

        public void OnSubscribe()
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
        }

        public void OnUnsubscribe()
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
        }

        public void OnClear()
        {
            GameServices.Deregister(this);
        }

        #endregion

        #region event handlers

        private void LevelLoadedEventHandler(object sender, LevelLoadedEventArgs e)
        {
            if (e.LevelConfig.BossEnabled)
            {
                if (GameServices.TryGetService(out EnemySpawner spawner) == true)
                {
                    spawner.SpawnEnded += EnemySpawnEndedEventHandler;
                }
                else
                {
                    throw new UnregisteredGameServiceAccessAttemptException(typeof(EnemySpawner));
                }

                _bossToSpawn = e.LevelConfig.Boss;
                _bossToSpawn.EnsureObjectPoolExistence();

                Active = true;
                BossIsAlive = true;
            }
            else
            {
                _bossToSpawn = null;
                Active = false;
                BossIsAlive = false;
            }
        }

        private void MainMenuLoadedEventHandler(object sender, EventArgs e)
        {
            if (GameServices.TryGetService(out EnemySpawner spawner) == true)
            {
                spawner.SpawnEnded -= EnemySpawnEndedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(EnemySpawner));
            }

            if (_spawnedBoss != null)
            {
                s_multiobjectPool.Access.ReleaseObject(_bossToSpawn.AnchorName, _spawnedBoss, () => true);
                _spawnedBoss = null;
            }

            if (_bossSpawningRoutine != null)
            {
                CoroutineRunner.StopRoutine(_bossSpawningRoutine);
                _bossSpawningRoutine = null;
            }

            _bossToSpawn = null;

            Active = false;
            BossIsAlive = false;
        }

        private void EnemySpawnEndedEventHandler(object sender, EventArgs e)
        {
             _bossSpawningRoutine = CoroutineRunner.RunRoutine(AwaitEnemiesDefeatThenSpawnBoss(sender as EnemySpawner));
        }

        #endregion

        private IEnumerator AwaitEnemiesDefeatThenSpawnBoss(EnemySpawner spawner)
        {
            while (spawner.AliveAmount > 0)
            {
                yield return null;
            }

            yield return new WaitForSeconds(BossSpawnDelay);

            _spawnedBoss = s_multiobjectPool.Access.GetObject(_bossToSpawn.AnchorName);
            _spawnedBoss.transform.SetLocalPositionAndRotation(BossSpawnPosition, BossSpawnRotation);

            _bossSpawnAlarm.PlayRandomAudioClip(Vector2.zero);

            if (_spawnedBoss.TryGetComponent(out IDestroyable d) == true)
            {
                _bossDestroyable = d;

                _bossDestroyable.Destroyed += (s, e) =>
                {
                    s_multiobjectPool.Access.ReleaseObject(_bossToSpawn.AnchorName, _spawnedBoss, () => true);
                    BossIsAlive = false;
                };
            }
            else
            {
                throw new MissingComponentException($"Spawned boss missing a mandatory component of type {typeof(IDestroyable)}!");
            }

            if (_spawnedBoss.TryGetComponent(out IEscapable e) == true)
            {
                _bossEscapable = e;
                _bossEscapable.BeginWatchForEscape(() => s_masterCameraHolder.Access.InsideViewport(_spawnedBoss.transform.position, BossEscapeDelta) == false);

                _bossEscapable.Escaped += (s, e) =>
                {
                    s_multiobjectPool.Access.ReleaseObject(_bossToSpawn.AnchorName, _spawnedBoss, () => true);

                    _spawnedBoss = null;
                    BossIsAlive = false;
                };
            }
            else
            {
                throw new MissingComponentException($"Spawned boss missing a mandatory component of type {typeof(IEscapable)}!");
            }

            BossSpawned?.Invoke(this, new EntitySpawnedEventArgs(_bossEscapable, _bossDestroyable));

            Active = false;
            BossIsAlive = true;
        }
    }
}