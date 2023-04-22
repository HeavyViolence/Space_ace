using SpaceAce.Architecture;
using SpaceAce.Auxiliary;
using SpaceAce.Main.Saving;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Levels
{
    public sealed class BestLevelsRunsStatisticsCollector : IInitializable, ISavable
    {
        public event EventHandler SavingRequested;

        private Dictionary<int, BestLevelRunStatistics> _statistics = new(LevelConfig.MaxLevelIndex);

        public string ID { get; }
        public string SaveName => "Best levels runs statistics";

        public BestLevelsRunsStatisticsCollector(string id)
        {
            if (StringID.IsValid(id) == false)
            {
                throw new InvalidStringIDException();
            }

            ID = id;
            _statistics.Add(1, BestLevelRunStatistics.Default);
        }

        public BestLevelRunStatistics GetStatistics(int levelIndex)
        {
            if (levelIndex < 1 || levelIndex > LevelConfig.MaxLevelIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(levelIndex),
                                                      $"Passed level index must be within the following range: [1, {LevelConfig.MaxLevelIndex}]!");
            }

            return _statistics.GetValueOrDefault(levelIndex);
        }

        #region interfaces

        public void OnInitialize()
        {
            GameServices.Register(this);
        }

        public void OnSubscribe()
        {
            if (GameServices.TryGetService(out SavingSystem system) == true)
            {
                system.Register(this);
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(SavingSystem));
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
            if (GameServices.TryGetService(out SavingSystem system) == true)
            {
                system.Deregister(this);
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(SavingSystem));
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

        public string GetState()
        {
            BestLevelsRunsStatisticsCollectorSavableData data = new(_statistics.Keys, _statistics.Values);

            return JsonUtility.ToJson(data);
        }

        public void SetState(string state)
        {
            var data = JsonUtility.FromJson<BestLevelsRunsStatisticsCollectorSavableData>(state);

            _statistics = new(data.Contents);
        }

        public override bool Equals(object obj) => Equals(obj as ISavable);

        public bool Equals(ISavable other) => other is not null && ID.Equals(other.ID);

        public override int GetHashCode() => ID.GetHashCode();

        #endregion

        private void LevelPassedEventHandler(object sender, LevelDataEventArgs e)
        {
            SavingRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}