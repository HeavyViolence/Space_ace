using System;
using System.Collections.Generic;

namespace SpaceAce.Architecture
{
    public sealed class GamePauser : IGameService
    {
        public event EventHandler GamePaused, GameResumed;

        private readonly HashSet<IPausable> _pausableEntities = new();

        public bool Paused { get; private set; } = false;

        public void Register(IPausable entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity), $"Attempted to register an empty {typeof(IPausable)} entity!");

            _pausableEntities.Add(entity);
        }

        public void Deregister(IPausable entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity), $"Attempted to deregister an empty {typeof(IPausable)} entity!");

            _pausableEntities.Remove(entity);
        }

        public void Pause()
        {
            if (Paused == true) return;

            foreach (var pausable in _pausableEntities) pausable.Pause();

            Paused = true;
            GamePaused?.Invoke(this, EventArgs.Empty);
        }

        public void Resume()
        {
            if (Paused == false) return;

            foreach (var pausable in _pausableEntities) pausable.Resume();

            Paused = false;
            GameResumed?.Invoke(this, EventArgs.Empty);
        }

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
        }
    }
}