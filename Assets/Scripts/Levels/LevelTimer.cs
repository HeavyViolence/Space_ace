using SpaceAce.Architecture;
using SpaceAce.Main;
using UnityEngine;
using System;
using SpaceAce.Auxiliary;

namespace SpaceAce.Levels
{
    public sealed class LevelTimer : IGameService, IUpdatable
    {
        private static readonly GameServiceFastAccess<GamePauser> s_gamePauser = new();

        private bool _gameLevelInProgress = false;
        private float _value = 0f;

        public int Seconds { get; private set; } = 0;
        public int Minutes { get; private set; } = 0;

        #region interfaces

        public void OnInitialize()
        {
            GameServices.Register(this);
        }

        public void OnSubscribe()
        {
            if (GameServices.TryGetService(out GameModeLoader loader) == true) loader.LevelLoaded += LevelLoadedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(GameModeLoader));

            if (GameServices.TryGetService(out LevelCompleter completer) == true) completer.LevelConcluded += LevelConcludedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(LevelCompleter));
        }

        public void OnUnsubscribe()
        {
            if (GameServices.TryGetService(out GameModeLoader loader) == true) loader.LevelLoaded -= LevelLoadedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(GameModeLoader));

            if (GameServices.TryGetService(out LevelCompleter completer) == true) completer.LevelConcluded -= LevelConcludedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(LevelCompleter));
        }

        public void OnClear()
        {
            GameServices.Deregister(this);
        }

        public void OnUpdate()
        {
            if (_gameLevelInProgress == true && s_gamePauser.Access.Paused == false)
            {
                _value += Time.deltaTime;

                if (_value > 1f)
                {
                    _value -= 1f;
                    Seconds++;

                    if (Seconds == AuxMath.SecondsPerMinute - 1)
                    {
                        Seconds = 0;
                        Minutes++;
                    }
                }
            }
        }

        #endregion

        private void LevelLoadedEventHandler(object sender, LevelLoadedEventArgs e)
        {
            _value = 0f;
            Seconds = 0;
            Minutes = 0;

            _gameLevelInProgress = true;
        }

        private void LevelConcludedEventHandler(object sender, EventArgs e)
        {
            _gameLevelInProgress = false;
        }
    }
}