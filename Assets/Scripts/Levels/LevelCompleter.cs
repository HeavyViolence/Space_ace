using SpaceAce.Architecture;
using SpaceAce.Gameplay.Players;
using SpaceAce.Gameplay.Spawning;
using SpaceAce.Main;
using System;
using System.Collections;

namespace SpaceAce.Levels
{
    public sealed class LevelCompleter : IInitializable
    {
        public event EventHandler<LevelDataEventArgs> LevelPassed, LevelFailed, LevelConcluded;

        private int _loadedLevelIndex = 0;

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
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(EnemySpawner));
            }
        }

        private void MainMenuLoadedEventHandler(object sender, EventArgs e)
        {
            _loadedLevelIndex = 0;

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
            var enemySpawner = sender as EnemySpawner;

            CoroutineRunner.RunRoutine(AwaitAllEnemiesDefeat());

            IEnumerator AwaitAllEnemiesDefeat()
            {
                while (enemySpawner.AliveAmount > 0)
                {
                    yield return null;
                }

                LevelPassed?.Invoke(this, new LevelDataEventArgs(_loadedLevelIndex));
                LevelConcluded?.Invoke(this, new LevelDataEventArgs(_loadedLevelIndex));
            }
        }

        #endregion
    }
}