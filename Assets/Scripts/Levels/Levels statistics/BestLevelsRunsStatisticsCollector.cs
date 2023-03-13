using SpaceAce.Architecture;
using SpaceAce.Auxiliary;
using SpaceAce.Main.Saving;
using System;
using System.Collections.Generic;

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

            for (int i = 1; i <= LevelConfig.MaxLevelIndex; i++)
            {
                _statistics.Add(i, BestLevelRunStatistics.Default);
            }
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

        public object GetState() => _statistics;

        public void SetState(object state)
        {
            if (state is null)
            {
                throw new EmptySavableStateEntryException(typeof(IEnumerable<KeyValuePair<int, BestLevelRunStatistics>>));
            }

            if (state is IEnumerable<KeyValuePair<int, BestLevelRunStatistics>> value)
            {
                _statistics = new(value);
            }
            else
            {
                throw new LoadedSavableEntityStateTypeMismatchException(state.GetType(),
                                                                        typeof(IEnumerable<KeyValuePair<int, BestLevelRunStatistics>>),
                                                                        GetType());
            }
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