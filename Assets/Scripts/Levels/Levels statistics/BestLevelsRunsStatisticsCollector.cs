using Newtonsoft.Json;
using SpaceAce.Architecture;
using SpaceAce.Gameplay.Damageables;
using SpaceAce.Gameplay.Movement;
using SpaceAce.Gameplay.Players;
using SpaceAce.Gameplay.Shooting;
using SpaceAce.Gameplay.Spawning;
using SpaceAce.Main;
using SpaceAce.Main.Saving;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Levels
{
    public sealed class BestLevelsRunsStatisticsCollector : IGameService, ISavable
    {
        public event EventHandler SavingRequested;

        private Dictionary<int, BestLevelRunStatistics> _statistics = new();

        private int _enemiesKilled = 0;

        private int _spaceDebrisDestroyed = 0;
        private int _spaceDebrisMissed = 0;

        private int _meteorsDestroyed = 0;
        private int _meteorsMissed = 0;

        private int _shotsFired = 0;
        private int _targetHits = 0;

        private float _damageReceived = 0f;
        private float _damageTaken = 0f;

        private float _damageDelivered = 0f;
        private float _damageDealt = 0f;

        private float _experienceEarned = 0f;
        private float _experienceLost = 0f;

        public string ID => "Best levels runs statistics";

        public BestLevelsRunsStatisticsCollector() { }

        public BestLevelRunStatistics GetStatistics(int levelIndex)
        {
            if (levelIndex < 1 || levelIndex > LevelConfig.MaxLevelIndex) throw new ArgumentOutOfRangeException(nameof(levelIndex));

            if (_statistics.TryGetValue(levelIndex, out var statistics) == true) return statistics;

            return BestLevelRunStatistics.Default;
        }

        #region interfaces

        public void OnInitialize()
        {
            GameServices.Register(this);
        }

        public void OnSubscribe()
        {
            if (GameServices.TryGetService(out SavingSystem system) == true) system.Register(this);
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(SavingSystem));

            if (GameServices.TryGetService(out LevelCompleter completer) == true) completer.LevelPassed += LevelPassedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(LevelCompleter));

            if (GameServices.TryGetService(out GameModeLoader loader) == true) loader.LevelLoaded += LevelLoadedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(GameModeLoader));

            if (GameServices.TryGetService(out EnemySpawner enemySpawner) == true) enemySpawner.EntitySpawned += EnemySpawnedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(EnemySpawner));

            if (GameServices.TryGetService(out SpaceDebrisSpawner debrisSpawner) == true) debrisSpawner.EntitySpawned += SpaceDebrisSpawnedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(SpaceDebrisSpawner));

            if (GameServices.TryGetService(out MeteorSpawner meteorSpawner) == true) meteorSpawner.EntitySpawned += MeteorSpawnedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(MeteorSpawner));

            if (GameServices.TryGetService(out Player player) == true) player.ShipSpawned += PlayerShipSpawnedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(Player));
        }

        public void OnUnsubscribe()
        {
            if (GameServices.TryGetService(out SavingSystem system) == true) system.Deregister(this);
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(SavingSystem));

            if (GameServices.TryGetService(out LevelCompleter completer) == true) completer.LevelPassed -= LevelPassedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(LevelCompleter));

            if (GameServices.TryGetService(out GameModeLoader loader) == true) loader.LevelLoaded -= LevelLoadedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(GameModeLoader));

            if (GameServices.TryGetService(out EnemySpawner spawner) == true) spawner.EntitySpawned -= EnemySpawnedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(EnemySpawner));

            if (GameServices.TryGetService(out SpaceDebrisSpawner debrisSpawner) == true) debrisSpawner.EntitySpawned -= SpaceDebrisSpawnedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(SpaceDebrisSpawner));

            if (GameServices.TryGetService(out MeteorSpawner meteorSpawner) == true) meteorSpawner.EntitySpawned -= MeteorSpawnedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(MeteorSpawner));

            if (GameServices.TryGetService(out Player player) == true) player.ShipSpawned -= PlayerShipSpawnedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(Player));
        }

        public void OnClear()
        {
            GameServices.Deregister(this);
        }

        public string GetState() => JsonConvert.SerializeObject(_statistics);

        public void SetState(string state) => _statistics = JsonConvert.DeserializeObject<Dictionary<int, BestLevelRunStatistics>>(state);

        public override bool Equals(object obj) => Equals(obj as ISavable);

        public bool Equals(ISavable other) => other is not null && ID.Equals(other.ID);

        public override int GetHashCode() => ID.GetHashCode();

        #endregion

        #region event handlers

        private void LevelPassedEventHandler(object sender, LevelDataEventArgs e)
        {
            if (GameServices.TryGetService(out LevelTimer timer) == true)
            {
                if (GameServices.TryGetService(out LevelRewardCollector rewardCollector) == true)
                {
                    BestLevelRunStatistics passedLevelNewStats = new(_enemiesKilled,
                                                                     _spaceDebrisDestroyed,
                                                                     _spaceDebrisMissed,
                                                                     _meteorsDestroyed,
                                                                     _meteorsMissed,
                                                                     _shotsFired,
                                                                     _targetHits,
                                                                     _damageReceived,
                                                                     _damageTaken,
                                                                     _damageDelivered,
                                                                     _damageDealt,
                                                                     rewardCollector.CreditReward,
                                                                     _experienceEarned,
                                                                     _experienceLost,
                                                                     (timer.Minutes, timer.Seconds));

                    _statistics.Add(e.LevelIndex, passedLevelNewStats);
                    SavingRequested?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    throw new UnregisteredGameServiceAccessAttemptException(typeof(LevelRewardCollector));
                }
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(LevelTimer));
            }
            
        }

        private void LevelLoadedEventHandler(object sender, LevelLoadedEventArgs e)
        {
            _enemiesKilled = 0;

            _spaceDebrisDestroyed = 0;
            _spaceDebrisMissed = 0;

            _meteorsDestroyed = 0;
            _meteorsMissed = 0;

            _shotsFired = 0;
            _targetHits = 0;

            _damageReceived = 0f;
            _damageTaken = 0f;

            _damageDelivered = 0f;
            _damageDealt = 0f;

            _experienceEarned = 0f;
            _experienceLost = 0f;
        }

        private void EnemySpawnedEventHandler(object sender, EntitySpawnedEventArgs e)
        {
            if (e.Entity.TryGetComponent(out IDestroyable destroyable) == true)
            {
                destroyable.Destroyed += (sender, args) =>
                {
                    _enemiesKilled++;
                    _experienceEarned += args.ExperienceEarned;
                    _experienceLost += args.ExperienceLost;
                };
            }
            else
            {
                throw new MissingComponentException(typeof(IDestroyable).ToString());
            }

            if (e.Entity.TryGetComponent(out IDamageable damageable) == true)
            {
                damageable.DamageReceived += (sender, args) =>
                {
                    _damageDelivered += args.DamageReceived;
                    _damageDealt += args.DamageTaken;
                };
            }
            else
            {
                throw new MissingComponentException(typeof(IDamageable).ToString());
            }
        }

        private void SpaceDebrisSpawnedEventHandler(object sender, EntitySpawnedEventArgs e)
        {
            if (e.Entity.TryGetComponent(out IDestroyable destroyable) == true)
            {
                destroyable.Destroyed += (sender, args) =>
                {
                    _spaceDebrisDestroyed++;
                    _experienceEarned += args.ExperienceEarned;
                    _experienceLost += args.ExperienceLost;
                };
            }
            else
            {
                throw new MissingComponentException(typeof(IDestroyable).ToString());
            }

            if (e.Entity.TryGetComponent(out IDamageable damageable) == true)
            {
                damageable.DamageReceived += (sender, args) =>
                {
                    _damageDelivered += args.DamageReceived;
                    _damageDealt += args.DamageTaken;
                };
            }
            else
            {
                throw new MissingComponentException(typeof(IDamageable).ToString());
            }

            if (e.Entity.TryGetComponent(out IEscapable escapable) == true) escapable.Escaped += (sender, args) => _spaceDebrisMissed++;
            else throw new MissingComponentException(typeof(IEscapable).ToString());
        }

        private void MeteorSpawnedEventHandler(object sender, EntitySpawnedEventArgs e)
        {
            if (e.Entity.TryGetComponent(out IDestroyable destroyable) == true)
            {
                destroyable.Destroyed += (sender, args) =>
                {
                    _meteorsDestroyed++;
                    _experienceEarned += args.ExperienceEarned;
                    _experienceLost += args.ExperienceLost;
                };
            }
            else
            {
                throw new MissingComponentException(typeof(IDestroyable).ToString());
            }

            if (e.Entity.TryGetComponent(out IDamageable damageable) == true)
            {
                damageable.DamageReceived += (sender, args) =>
                {
                    _damageDelivered += args.DamageReceived;
                    _damageDealt += args.DamageTaken;
                };
            }
            else
            {
                throw new MissingComponentException(typeof(IDamageable).ToString());
            }

            if (e.Entity.TryGetComponent( out IEscapable escapable) == true) escapable.Escaped += (sender, args) => _meteorsMissed++;
            else throw new MissingComponentException(typeof(IEscapable).ToString());
        }

        private void PlayerShipSpawnedEventHandler(object sender, PlayerShipSpawnedEventArgs e)
        {
            if (e.Ship.TryGetComponent(out IGunner gunner) == true)
            {
                gunner.GunFired += (sender, args) => _shotsFired++;
                gunner.TargetHit += (snder, args) => _targetHits++;
            }
            else
            {
                throw new MissingComponentException(typeof(IGunner).ToString());
            }

            if (e.Ship.TryGetComponent(out IDamageable damageable) == true)
            {
                damageable.DamageReceived += (sender, args) =>
                {
                    _damageReceived += args.DamageReceived;
                    _damageTaken += args.DamageTaken;
                };
            }
            else
            {
                throw new MissingComponentException(typeof(IDamageable).ToString());
            }
        }

        #endregion
    }
}