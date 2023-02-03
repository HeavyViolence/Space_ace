using SpaceAce.Architecture;
using System;
using System.Collections.Generic;

namespace SpaceAce.Main
{
    public sealed class GameModeLoader : IInitializable
    {
        public event EventHandler MainMeunuLoaded;
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
            if (GameServices.TryGetService(out ScreenFader fader))
            {
                fader.FadedOut += delegate
                {
                    MainMeunuLoaded?.Invoke(this, EventArgs.Empty);
                };

                fader.PerformScreenFading();
            }
        }

        public void LoadGameLevel(EnemyType type, LevelDifficulty difficulty)
        {
            if (GameServices.TryGetService(out ScreenFader fader))
            {
                fader.FadedOut += delegate
                {
                    bool necessaryLevelConfigFound = false;

                    foreach (var levelConfig in _levelConfigs)
                    {
                        if (levelConfig.VerifyIdentityMatch(type, difficulty) == true)
                        {
                            necessaryLevelConfigFound = true;
                            LevelLoaded?.Invoke(this, new LevelLoadedEventArgs(levelConfig));

                            break;
                        }
                    }

                    if (necessaryLevelConfigFound == false)
                    {
                        throw new LevelLoadFailedException(type, difficulty);
                    }
                };

                fader.PerformScreenFading();
            }
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

            MainMeunuLoaded = null;
            LevelLoaded = null;
        }

        #endregion
    }
}