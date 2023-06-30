using SpaceAce.Architecture;
using SpaceAce.Gameplay.Players;
using SpaceAce.Gameplay.Spawning;
using SpaceAce.Main;
using System;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Levels
{
    public sealed class LevelCompleter : IGameService
    {
        public event EventHandler<LevelDataEventArgs> LevelPassed, LevelFailed, LevelConcluded;

        private int _loadedLevelIndex = 0;
        private Coroutine _levelConcluderRoutine;

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
                loader.MainMenuLoadingStarted += MainMenuLoadingStartedEventHandler;
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
                loader.MainMenuLoadingStarted -= MainMenuLoadingStartedEventHandler;
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
            _loadedLevelIndex = e.LevelConfig.LevelIndex;

            if (GameServices.TryGetService(out Player player) == true)
            {
                player.ShipSpawned += PlayerShipSpawnedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(Player));
            }

            if (GameServices.TryGetService(out EnemySpawner spawner) == true)
            {
                spawner.SpawnEnded += EnemySpawnEndedEventHandler;
                spawner.EntitySpawned += EnemySpawnedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(EnemySpawner));
            }
        }

        private void MainMenuLoadingStartedEventHandler(object sender, LoadingStartedEventArgs e)
        {
            if (_levelConcluderRoutine != null)
            {
                CoroutineRunner.StopRoutine(_levelConcluderRoutine);
                _levelConcluderRoutine = null;
            }
        }

        private void MainMenuLoadedEventHandler(object sender, EventArgs e)
        {
            _loadedLevelIndex = 0;

            if (_levelConcluderRoutine != null)
            {
                CoroutineRunner.StopRoutine(_levelConcluderRoutine);
                _levelConcluderRoutine = null;
            }

            if (GameServices.TryGetService(out Player player) == true)
            {
                player.ShipSpawned -= PlayerShipSpawnedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(Player));
            }

            if (GameServices.TryGetService(out EnemySpawner spawner) == true)
            {
                spawner.SpawnEnded -= EnemySpawnEndedEventHandler;
                spawner.EntitySpawned -= EnemySpawnedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(EnemySpawner));
            }
        }

        private void PlayerShipSpawnedEventHandler(object sender, PlayerShipSpawnedEventArgs e)
        {
            e.Destroyable.Destroyed += (s, e) =>
            {
                LevelFailed?.Invoke(this, new LevelDataEventArgs(_loadedLevelIndex));
                LevelConcluded?.Invoke(this, new LevelDataEventArgs(_loadedLevelIndex));
            };
        }

        private void EnemySpawnEndedEventHandler(object sender, EventArgs e)
        {
            _levelConcluderRoutine = CoroutineRunner.RunRoutine(AwaitEnemiesDefeatThenConcludeLevel(sender as EnemySpawner));
        }

        private void EnemySpawnedEventHandler(object sender, EntitySpawnedEventArgs e)
        {
            e.Escapable.Escaped += (s, e) =>
            {
                LevelFailed?.Invoke(this, new LevelDataEventArgs(_loadedLevelIndex));
                LevelConcluded?.Invoke(this, new LevelDataEventArgs(_loadedLevelIndex));
            };
        }

        #endregion

        IEnumerator AwaitEnemiesDefeatThenConcludeLevel(EnemySpawner enemySpawner)
        {
            while (enemySpawner.ALiveCount > 0)
            {
                yield return null;
            }

            if (GameServices.TryGetService(out BossSpawner bossSpawner) == true)
            {
                while (bossSpawner.Active)
                {
                    yield return null;
                }

                yield return null;

                while (bossSpawner.BossIsAlive)
                {
                    yield return null;
                }
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(BossSpawner));
            }

            LevelPassed?.Invoke(this, new LevelDataEventArgs(_loadedLevelIndex));
            LevelConcluded?.Invoke(this, new LevelDataEventArgs(_loadedLevelIndex));
        }
    }
}