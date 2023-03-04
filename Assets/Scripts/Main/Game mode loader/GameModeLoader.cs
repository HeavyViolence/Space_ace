using SpaceAce.Architecture;
using SpaceAce.Levelry;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Main
{
    public sealed class GameModeLoader : IInitializable, IRunnable
    {
        private const float GameModeLoadingDelay = 1f;

        public event EventHandler<LoadingStartedEventArgs> MainMenuLoadingStarted;
        public event EventHandler MainMenuLoaded;

        public event EventHandler<LoadingStartedEventArgs> LevelLoadingStarted;
        public event EventHandler<LevelLoadedEventArgs> LevelLoaded;

        private readonly HashSet<LevelConfig> _levelConfigs;

        public GameModeLoader(IEnumerable<LevelConfig> levelConfigs)
        {
            if (levelConfigs is null)
            {
                throw new ArgumentNullException(nameof(levelConfigs), $"Attempted to pass an empty {nameof(LevelConfig)} collection!");
            }

            _levelConfigs = new(levelConfigs);
        }

        public void LoadMainMenu()
        {
            CoroutineRunner.RunRoutine(MainMenuLoader());

            IEnumerator MainMenuLoader()
            {
                MainMenuLoadingStarted?.Invoke(this, new LoadingStartedEventArgs(GameModeLoadingDelay));

                yield return new WaitForSeconds(GameModeLoadingDelay);

                MainMenuLoaded?.Invoke(this, EventArgs.Empty);
            }
        }

        public void LoadLevel(int levelIndex)
        {
            if (levelIndex < LevelConfig.MinLevelIndex || levelIndex > LevelConfig.MaxLevelIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(levelIndex), "Attempted to pass an invalid level index!");
            }

            CoroutineRunner.RunRoutine(GameLevelLoader());

            IEnumerator GameLevelLoader()
            {
                LevelLoadingStarted?.Invoke(this, new LoadingStartedEventArgs(GameModeLoadingDelay));

                yield return new WaitForSeconds(GameModeLoadingDelay);

                bool necessaryLevelConfigFound = false;

                foreach (var config in _levelConfigs)
                {
                    if (config.LevelIndex == levelIndex)
                    {
                        necessaryLevelConfigFound = true;
                        LevelLoaded?.Invoke(this, new LevelLoadedEventArgs(config));

                        break;
                    }
                }

                if (necessaryLevelConfigFound == false)
                {
                    throw new LevelLoadFailedException(levelIndex);
                }
            }
        }

        public LevelConfig GetLevelConfig(int levelIndex)
        {
            foreach (var config in _levelConfigs)
            {
                if (config.LevelIndex == levelIndex)
                {
                    return config;
                }
            }

            throw new Exception($"{nameof(LevelConfig)} for level #{levelIndex} is absent from the collection!");
        }

        #region interfaces

        public void OnInitialize()
        {
            GameServices.Register(this);
        }

        public void OnSubscribe()
        {

        }

        public void OnUnsubscribe()
        {

        }

        public void OnClear()
        {
            GameServices.Deregister(this);

            MainMenuLoadingStarted = null;
            MainMenuLoaded = null;

            LevelLoadingStarted = null;
            LevelLoaded = null;
        }

        public void OnRun()
        {
            MainMenuLoaded(this, EventArgs.Empty);
        }

        #endregion
    }
}