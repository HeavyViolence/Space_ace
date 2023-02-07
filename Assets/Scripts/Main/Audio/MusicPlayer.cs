using SpaceAce.Architecture;
using SpaceAce.Main.Saving;
using System;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Main.Audio
{
    public sealed class MusicPlayer : IInitializable, ISavable, IRunnable
    {
        public const float MinPlaybackInterval = 5f;
        public const float MaxPlaybackInterval = 300f;
        public const float DefaultPlaybackInterval = 10f;

        public event EventHandler SavingRequested;

        private readonly AudioCollection _music;
        private readonly GameServiceFastAccess<AudioPlayer> _audioPlayer = new();

        private Coroutine _musicRoutine = null;
        private AudioAccess _currentTrackAccess = null;

        public string ID { get; private set; }
        public float PlaybackInterval { get; private set; }
        public bool IsPlaying => _musicRoutine != null;

        public MusicPlayer(string id, AudioCollection music)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id), "Attempted to set an invalid ID!");
            }

            ID = id;

            if (music == null)
            {
                throw new ArgumentNullException(nameof(music), $"Attempted to set an empty {typeof(AudioCollection)}!");
            }

            _music = music;
        }

        public void Play()
        {
            if (IsPlaying == false)
            {
                _musicRoutine = CoroutineRunner.RunRoutine(PlayMusicForever());
            }
        }

        public void Stop()
        {
            if (IsPlaying)
            {
                CoroutineRunner.StopRoutine(_musicRoutine);
                _musicRoutine = null;

                _audioPlayer.Access.InterruptPlay(_currentTrackAccess.ID);
            }
        }

        private IEnumerator PlayMusicForever()
        {
            while (true)
            {
                _currentTrackAccess = _music.PlayRandomAudioClip(Vector2.zero);

                yield return new WaitForSeconds(PlaybackInterval + _currentTrackAccess.PlaybackDuration);
            }
        }

        public void SetPlaybackInterval(float value)
        {
            PlaybackInterval = Mathf.Clamp(value, MinPlaybackInterval, MaxPlaybackInterval);
            SavingRequested?.Invoke(this, EventArgs.Empty);
        }

        public void SetDefaultPlaybackInterval() => SetPlaybackInterval(DefaultPlaybackInterval);

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
        }

        public void OnUnsubscribe()
        {
            if (GameServices.TryGetService(out SavingSystem system) == true)
            {
                system.Deregister(this);
            }
        }

        public void OnClear()
        {
            GameServices.Deregister(this);
        }

        public void OnRun()
        {
            Play();
        }

        public object GetState() => PlaybackInterval;

        public void SetState(object state)
        {
            if (state is float value)
            {
                PlaybackInterval = value;
            }
            else
            {
                throw new LoadedSavableEntityStateTypeMismatchException(state.GetType(), typeof(float), GetType());
            }
        }

        public override bool Equals(object obj) => Equals(obj as ISavable);

        public bool Equals(ISavable other) => other is not null && other.ID.Equals(ID);

        public override int GetHashCode() => ID.GetHashCode();

        #endregion
    }
}
