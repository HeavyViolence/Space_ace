using SpaceAce.Architecture;
using SpaceAce.Main;
using System;

namespace SpaceAce.Levels
{
    public sealed class LevelCompleter : IInitializable
    {
        public event EventHandler<LevelDataEventArgs> LevelPassed, LevelFailed;
        public event EventHandler LevelConcluded;

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
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(GameModeLoader));
            }
        }

        public void OnUnsubscribe()
        {
            if (GameServices.TryGetService(out GameModeLoader loader) == true)
            {
                loader.LevelLoaded -= LevelLoadedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(GameModeLoader));
            }
        }

        public void OnClear()
        {
            GameServices.Deregister(this);
        }

        #endregion

        private void LevelLoadedEventHandler(object sender, LevelLoadedEventArgs e)
        {

        }
    }
}