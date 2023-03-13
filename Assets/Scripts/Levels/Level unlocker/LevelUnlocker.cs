using SpaceAce.Architecture;
using SpaceAce.Auxiliary;
using SpaceAce.Main.Saving;
using System;
using System.Collections.Generic;

namespace SpaceAce.Levels
{
    public sealed class LevelUnlocker: IInitializable, ISavable
    {
        public event EventHandler SavingRequested;

        private HashSet<int> _passedLevels = new();
        private HashSet<int> _unlockedLevels = new();

        public string ID { get; }
        public string SaveName => "Levels progress";

        public LevelUnlocker(string id)
        {
            if (StringID.IsValid(id) == false)
            {
                throw new InvalidStringIDException();
            }

            ID = id;
            _unlockedLevels.Add(1);
        }

        public bool IsLevelUnlocked(int levelIndex) => _unlockedLevels.Contains(levelIndex);

        public bool IsLevelPassed(int levelIndex) => _passedLevels.Contains(levelIndex);

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

        public object GetState() => new LevelUnlockerSavableData(_passedLevels, _unlockedLevels);

        public void SetState(object state)
        {
            if (state is null)
            {
                throw new EmptySavableStateEntryException(typeof(LevelUnlockerSavableData));
            }

            if (state is LevelUnlockerSavableData value)
            {
                if (value.PassedLevels is not null)
                {
                    _passedLevels = new(value.PassedLevels);
                }
                
                if (value.UnlockedLevels is not null)
                {
                    _unlockedLevels = new(value.UnlockedLevels);
                }
            }
            else
            {
                throw new LoadedSavableEntityStateTypeMismatchException(state.GetType(),
                                                                        typeof(LevelUnlockerSavableData),
                                                                        GetType());
            }
        }

        public override bool Equals(object obj) => Equals(obj as ISavable);

        public bool Equals(ISavable other) => other is not null && other.ID.Equals(ID);

        public override int GetHashCode() => ID.GetHashCode();

        #endregion

        private void LevelPassedEventHandler(object sender, LevelDataEventArgs e)
        {
            _passedLevels.Add(e.LevelIndex);
            _unlockedLevels.Add(e.LevelIndex + 1);

            SavingRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}