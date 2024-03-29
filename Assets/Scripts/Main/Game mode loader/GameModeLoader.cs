using SpaceAce.Architecture;
using SpaceAce.Levels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Main
{
    public enum GameMode
    {
        Booting,
        MainMenu,
        MainMenuLoading,
        Level,
        LevelLoading,
        LevelPassed,
        LevelFailed
    }

    public sealed class GameModeLoader : IGameService, IRunnable
    {
        private const float GameModeLoadingDelay = 1.5f;

        public event EventHandler<LoadingStartedEventArgs> MainMenuLoadingStarted;
        public event EventHandler MainMenuLoaded;

        public event EventHandler<LoadingStartedEventArgs> LevelLoadingStarted;
        public event EventHandler<LevelLoadedEventArgs> LevelLoaded;

        private readonly HashSet<LevelConfig> _levelConfigs;

        public GameMode GameMode { get; private set; } = GameMode.Booting;

        public GameModeLoader(IEnumerable<LevelConfig> levelConfigs)
        {
            if (levelConfigs is null) throw new ArgumentNullException(nameof(levelConfigs));
            _levelConfigs = new(levelConfigs);
        }

        public void LoadMainMenu()
        {
            CoroutineRunner.RunRoutine(MainMenuLoader());

            IEnumerator MainMenuLoader()
            {
                MainMenuLoadingStarted?.Invoke(this, new LoadingStartedEventArgs(GameModeLoadingDelay));
                GameMode = GameMode.MainMenuLoading;

                yield return new WaitForSeconds(GameModeLoadingDelay);

                MainMenuLoaded?.Invoke(this, EventArgs.Empty);
                GameMode = GameMode.MainMenu;
            }
        }

        public void LoadLevel(int levelIndex)
        {
            if (levelIndex < LevelConfig.MinLevelIndex ||
                levelIndex > LevelConfig.MaxLevelIndex) throw new ArgumentOutOfRangeException(nameof(levelIndex));

            CoroutineRunner.RunRoutine(GameLevelLoader());

            IEnumerator GameLevelLoader()
            {
                LevelLoadingStarted?.Invoke(this, new LoadingStartedEventArgs(GameModeLoadingDelay));
                GameMode = GameMode.LevelLoading;

                yield return new WaitForSeconds(GameModeLoadingDelay);

                bool necessaryLevelConfigFound = false;

                foreach (var config in _levelConfigs)
                {
                    if (config.LevelIndex == levelIndex)
                    {
                        necessaryLevelConfigFound = true;

                        LevelLoaded?.Invoke(this, new LevelLoadedEventArgs(config));
                        GameMode = GameMode.Level;

                        break;
                    }
                }

                if (necessaryLevelConfigFound == false) throw new LevelLoadFailedException(levelIndex);
            }
        }

        public LevelConfig GetLevelConfig(int levelIndex)
        {
            foreach (var config in _levelConfigs) if (config.LevelIndex == levelIndex) return config;

            throw new Exception($"{nameof(LevelConfig)} for level #{levelIndex} is absent from the collection!");
        }

        #region interfaces

        public void OnInitialize()
        {
            GameServices.Register(this);
        }

        public void OnSubscribe()
        {
            if (GameServices.TryGetService(out LevelCompleter completer) == true)
            {
                completer.LevelPassed += (s, e) => GameMode = GameMode.LevelPassed;
                completer.LevelFailed += (s, e) => GameMode = GameMode.LevelFailed;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(LevelCompleter));
            }
        }

        public void OnUnsubscribe()
        {
            if (GameServices.TryGetService(out LevelCompleter completer) == true)
            {
                completer.LevelPassed -= (s, e) => GameMode = GameMode.LevelPassed;
                completer.LevelFailed -= (s, e) => GameMode = GameMode.LevelFailed;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(LevelCompleter));
            }
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
            GameMode = GameMode.MainMenu;
        }

        #endregion
    }
}