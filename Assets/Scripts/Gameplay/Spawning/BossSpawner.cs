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
        private GameObject _boss;

        private Coroutine _bossSpawningRoutine;

        public bool Active { get; private set; } = false;
        public bool BossIsAlive { get; private set; } = false;

        public BossSpawner(AudioCollection bossSpawnAlarm)
        {
            if (bossSpawnAlarm == null) throw new ArgumentNullException(nameof(bossSpawnAlarm));
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
                if (GameServices.TryGetService(out EnemySpawner spawner) == true) spawner.SpawnEnded += EnemySpawnEndedEventHandler;
                else throw new UnregisteredGameServiceAccessAttemptException(typeof(EnemySpawner));

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
            if (GameServices.TryGetService(out EnemySpawner spawner) == true) spawner.SpawnEnded -= EnemySpawnEndedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(EnemySpawner));

            if (_boss != null)
            {
                s_multiobjectPool.Access.ReleaseObject(_bossToSpawn.AnchorName, _boss, () => true);
                _boss = null;
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
            while (spawner.ALiveCount > 0) yield return null;

            yield return new WaitForSeconds(BossSpawnDelay);

            _boss = s_multiobjectPool.Access.GetObject(_bossToSpawn.AnchorName);
            _boss.transform.SetLocalPositionAndRotation(BossSpawnPosition, BossSpawnRotation);

            _bossSpawnAlarm.PlayRandomAudioClip(Vector2.zero);

            if (_boss.TryGetComponent(out IDestroyable destroyable) == true)
            {
                destroyable.Destroyed += (s, e) =>
                {
                    s_multiobjectPool.Access.ReleaseObject(_bossToSpawn.AnchorName, _boss, () => true);
                    BossIsAlive = false;
                };
            }
            else
            {
                throw new MissingComponentException($"Spawned boss missing a mandatory component of type {typeof(IDestroyable)}!");
            }

            if (_boss.TryGetComponent(out IEscapable escapable) == true)
            {
                escapable.StartWatchingForEscape(() => s_masterCameraHolder.Access.InsideViewport(_boss.transform.position, BossEscapeDelta) == false);

                escapable.Escaped += (s, e) =>
                {
                    s_multiobjectPool.Access.ReleaseObject(_bossToSpawn.AnchorName, _boss, () => true);

                    _boss = null;
                    BossIsAlive = false;
                };
            }
            else
            {
                throw new MissingComponentException($"Spawned boss missing a mandatory component of type {typeof(IEscapable)}!");
            }

            BossSpawned?.Invoke(this, new(_boss));

            Active = false;
            BossIsAlive = true;
        }
    }
}