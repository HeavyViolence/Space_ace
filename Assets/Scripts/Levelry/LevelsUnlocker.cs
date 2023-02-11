using SpaceAce.Architecture;
using SpaceAce.Auxiliary;
using SpaceAce.Main;
using SpaceAce.Main.Saving;
using System;
using System.Collections.Generic;

namespace SpaceAce.Levelry
{
    public sealed class LevelsUnlocker: IInitializable, ISavable
    {
        public event EventHandler SavingRequested;

        private HashSet<int> _passedLevelsIndices = new();
        private HashSet<int> _unlockedLevelsIndices = new();

        public string ID { get; }
        public string SaveName => "Levels progress";

        public LevelsUnlocker(string id)
        {
            if (StringID.IsValid(id) == false)
            {
                throw new InvalidStringIDException();
            }

            ID = id;
            _unlockedLevelsIndices.Add(0);
        }

        public bool IsLevelUnlocked(EnemyType type, LevelDifficulty difficulty)
        {
            int levelIndex = AuxMath.GetLevelIndex(type, difficulty);

            return _unlockedLevelsIndices.Contains(levelIndex);
        }

        public bool IsLevelPassed(EnemyType type, LevelDifficulty difficulty)
        {
            int levelIndex = AuxMath.GetLevelIndex(type, difficulty);

            return _passedLevelsIndices.Contains(levelIndex);
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

        public object GetState() => new LevelsUnlockerSavableData(_passedLevelsIndices,
                                                                  _unlockedLevelsIndices);

        public void SetState(object state)
        {
            if (state is LevelsUnlockerSavableData value)
            {
                if (value.PassedLevelsIndices is not null)
                {
                    _passedLevelsIndices = new(value.PassedLevelsIndices);
                }
                
                if (value.UnlockedLevelsIndices is not null)
                {
                    _unlockedLevelsIndices = new(value.UnlockedLevelsIndices);
                }
            }
            else
            {
                throw new LoadedSavableEntityStateTypeMismatchException(state.GetType(),
                                                                        typeof(LevelsUnlockerSavableData),
                                                                        GetType());
            }
        }

        public override bool Equals(object obj) => Equals(obj as ISavable);

        public bool Equals(ISavable other) => other is not null && other.ID.Equals(ID);

        public override int GetHashCode() => ID.GetHashCode();

        #endregion

        private void LevelPassedEventHandler(object sender, LevelDataEventArgs e)
        {
            int passedLevelIndex = AuxMath.GetLevelIndex(e.EnemyType, e.Difficulty);
            int unlockedLevelIndex = passedLevelIndex + 1;

            _passedLevelsIndices.Add(passedLevelIndex);
            _unlockedLevelsIndices.Add(unlockedLevelIndex);

            SavingRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}