using SpaceAce.Architecture;
using SpaceAce.Gameplay.Inventories;
using SpaceAce.Gameplay.Spawning;
using SpaceAce.Main;
using System;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Levels
{
    public sealed class LevelRewardCollector : IGameService, ISponsorshipUser
    {
        private static readonly GameServiceFastAccess<GameModeLoader> s_gameModeLoader = new();
        private static readonly GameServiceFastAccess<GamePauser> s_gamePauser = new();

        public event EventHandler<LevelCreditRewardChangeEventArgs> CreditRewardChanged;
        public event EventHandler<LevelExperienceRewardChangedEventArgs> ExperienceRewardChanged;
        public event EventHandler<LevelRewardDispensedEventArgs> RewardDispensed;

        private Coroutine _sponsorship = null;
        private float _experienceToCreditsConversionRate = 0f;

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

            if (GameServices.TryGetService(out EnemySpawner enemySpawner) == true) enemySpawner.EntitySpawned += EntitySpawnedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(EnemySpawner));

            if (GameServices.TryGetService(out BossSpawner bossSpawner) == true) bossSpawner.BossSpawned += EntitySpawnedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(BossSpawner));

            if (GameServices.TryGetService(out MeteorSpawner meteorSpawner) == true) meteorSpawner.EntitySpawned += EntitySpawnedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(MeteorSpawner));

            if (GameServices.TryGetService(out SpaceDebrisSpawner debrisSpawner) == true) debrisSpawner.EntitySpawned += EntitySpawnedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(SpaceDebrisSpawner));

            if (GameServices.TryGetService(out LevelCompleter completer) == true) completer.LevelPassed += LevelPassedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(LevelCompleter));

            if (GameServices.TryGetService(out SpecialEffectsMediator mediator) == true) mediator.Register(this);
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(SpecialEffectsMediator));
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

            if (GameServices.TryGetService(out EnemySpawner spawner) == true) spawner.EntitySpawned -= EntitySpawnedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(EnemySpawner));

            if (GameServices.TryGetService(out BossSpawner bossSpawner) == true) bossSpawner.BossSpawned -= EntitySpawnedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(BossSpawner));

            if (GameServices.TryGetService(out MeteorSpawner meteorSpawner) == true) meteorSpawner.EntitySpawned -= EntitySpawnedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(MeteorSpawner));

            if (GameServices.TryGetService(out SpaceDebrisSpawner debrisSpawner) == true) debrisSpawner.EntitySpawned -= EntitySpawnedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(SpaceDebrisSpawner));

            if (GameServices.TryGetService(out LevelCompleter completer) == true) completer.LevelPassed -= LevelPassedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(LevelCompleter));

            if (GameServices.TryGetService(out SpecialEffectsMediator mediator) == true) mediator.Deregister(this);
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(SpecialEffectsMediator));
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
                float oldExperienceReward = ExperienceReward;
                float newExperienceReward = oldExperienceReward + e.EarnedExperience;

                ExperienceReward += e.EarnedExperience;
                ExperienceRewardChanged?.Invoke(this, new(oldExperienceReward, newExperienceReward));

                if (_sponsorship != null)
                {
                    int oldCreditReward = CreditReward;
                    int newCreditReward = oldCreditReward + (int)(e.EarnedExperience * _experienceToCreditsConversionRate);

                    CreditReward = newCreditReward;
                    CreditRewardChanged?.Invoke(this, new(oldCreditReward, newCreditReward));
                }
            };
        }

        private void LevelPassedEventHandler(object sender, LevelDataEventArgs e)
        {
            RewardDispensed?.Invoke(this, new(CreditReward, ExperienceReward));
        }

        #endregion

        public bool Use(Sponsorship sponsorship)
        {
            if (sponsorship is null) throw new ArgumentNullException(nameof(sponsorship));

            if (_sponsorship == null)
            {
                _sponsorship = CoroutineRunner.RunRoutine(ApplySponsorship(sponsorship));
                return true;
            }

            return false;
        }

        private IEnumerator ApplySponsorship(Sponsorship sponsorship)
        {
            _experienceToCreditsConversionRate = sponsorship.ExperienceToCreditsConversionRate;
            float timer = 0f;

            while (timer < sponsorship.Duration)
            {
                timer += Time.deltaTime;

                if (s_gameModeLoader.Access.GameState != GameState.Level)
                {
                    _experienceToCreditsConversionRate = 0f;
                    _sponsorship = null;

                    yield break;
                }

                yield return null;
                while (s_gamePauser.Access.Paused == true) yield return null;
            }

            _experienceToCreditsConversionRate = 0f;
            _sponsorship = null;
        }
    }
}