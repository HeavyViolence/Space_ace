using SpaceAce.Architecture;
using SpaceAce.Auxiliary;
using SpaceAce.Main.Saving;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Levels
{
    public sealed class LevelUnlocker: IGameService, ISavable
    {
        public event EventHandler SavingRequested;

        private HashSet<int> _passedLevels = new();
        private HashSet<int> _unlockedLevels = new();

        public string ID { get; }
        public string SaveName => "Levels progress";

        public LevelUnlocker(string id)
        {
            if (StringID.IsValid(id) == false) throw new InvalidStringIDException();

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

        public string GetState()
        {
            LevelUnlockerSavableData state = new(_passedLevels, _unlockedLevels);

            return JsonUtility.ToJson(state);
        }

        public void SetState(string state)
        {
            var data = JsonUtility.FromJson<LevelUnlockerSavableData>(state);

            if (data.PassedLevels is not null)
            {
                _passedLevels = new(data.PassedLevels);
            }

            if (data.UnlockedLevels is not null)
            {
                _unlockedLevels = new(data.UnlockedLevels);
            }
        }

        public override bool Equals(object obj) => Equals(obj as ISavable);

        public bool Equals(ISavable other) => other is not null && other.ID.Equals(ID);

        public override int GetHashCode() => ID.GetHashCode();

        #endregion

        private void LevelPassedEventHandler(object sender, LevelDataEventArgs e)
        {
            _passedLevels.Add(e.LevelIndex);

            if (e.LevelIndex < 5) _unlockedLevels.Add(e.LevelIndex + 1);

            SavingRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}