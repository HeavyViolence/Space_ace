using SpaceAce.Architecture;
using SpaceAce.Gameplay.Spawning;
using SpaceAce.Main;
using System;

namespace SpaceAce.Levels
{
    public sealed class LevelRewardCollector : IGameService
    {
        public event EventHandler<LevelCreditRewardChangeEventArgs> CreditRewardChanged;
        public event EventHandler<LevelExperienceRewardChangedEventArgs> ExperienceRewardChanged;
        public event EventHandler<LevelRewardDispensedEventArgs> RewardDispensed;

        public int CreditReward { get; private set; }
        public float ExperienceReward { get; private set; }

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
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(GameModeLoader));
            }

            if (GameServices.TryGetService(out EnemySpawner enemySpawner) == true)
            {
                enemySpawner.EntitySpawned += EntitySpawnedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(EnemySpawner));
            }

            if (GameServices.TryGetService(out BossSpawner bossSpawner) == true)
            {
                bossSpawner.BossSpawned += EntitySpawnedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(BossSpawner));
            }

            if (GameServices.TryGetService(out MeteorSpawner meteorSpawner) == true)
            {
                meteorSpawner.EntitySpawned += EntitySpawnedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(MeteorSpawner));
            }

            if (GameServices.TryGetService(out SpaceDebrisSpawner debrisSpawner) == true)
            {
                debrisSpawner.EntitySpawned += EntitySpawnedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(SpaceDebrisSpawner));
            }

            if (GameServices.TryGetService(out LevelCompleter completer) == true)
            {
                completer.LevelPassed += LevelPassedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(LevelCompleter));
            }
        }

        public void OnUnsubscribe()
        {
            if (GameServices.TryGetService(out GameModeLoader loader) == true)
            {
                loader.LevelLoaded -= LevelLoadedEventHandler;
                loader.MainMenuLoadingStarted -= MainMenuLoadingStartedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(GameModeLoader));
            }

            if (GameServices.TryGetService(out EnemySpawner spawner) == true)
            {
                spawner.EntitySpawned -= EntitySpawnedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(EnemySpawner));
            }

            if (GameServices.TryGetService(out BossSpawner bossSpawner) == true)
            {
                bossSpawner.BossSpawned -= EntitySpawnedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(BossSpawner));
            }

            if (GameServices.TryGetService(out MeteorSpawner meteorSpawner) == true)
            {
                meteorSpawner.EntitySpawned -= EntitySpawnedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(MeteorSpawner));
            }

            if (GameServices.TryGetService(out SpaceDebrisSpawner debrisSpawner) == true)
            {
                debrisSpawner.EntitySpawned -= EntitySpawnedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(SpaceDebrisSpawner));
            }

            if (GameServices.TryGetService(out LevelCompleter completer) == true)
            {
                completer.LevelPassed -= LevelPassedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(LevelCompleter));
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
            CreditReward = e.LevelConfig.CreditReward;
            ExperienceReward = e.LevelConfig.ExperienceReward;
        }

        private void MainMenuLoadingStartedEventHandler(object sender, LoadingStartedEventArgs e)
        {
            CreditReward = 0;
            ExperienceReward = 0;
        }

        private void EntitySpawnedEventHandler(object sender, EntitySpawnedEventArgs e)
        {
            e.Destroyable.Destroyed += (s, e) =>
            {
                float oldReward = ExperienceReward;
                float newReward = oldReward + e.EarnedExperience;

                ExperienceReward += e.EarnedExperience;
                ExperienceRewardChanged?.Invoke(this, new LevelExperienceRewardChangedEventArgs(oldReward, newReward));
            };
        }

        private void LevelPassedEventHandler(object sender, LevelDataEventArgs e)
        {
            RewardDispensed?.Invoke(this, new(CreditReward, ExperienceReward));
        }

        #endregion
    }
}