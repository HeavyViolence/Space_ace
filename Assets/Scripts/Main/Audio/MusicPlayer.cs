using Newtonsoft.Json;
using SpaceAce.Architecture;
using SpaceAce.Main.Saving;
using System;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Main.Audio
{
    public sealed class MusicPlayer : IGameService, ISavable, IRunnable
    {
        public const float MinPlaybackInterval = 5f;
        public const float MaxPlaybackInterval = 300f;
        public const float DefaultPlaybackInterval = 10f;

        public event EventHandler SavingRequested;

        private readonly AudioCollection _music;
        private readonly GameServiceFastAccess<AudioPlayer> _audioPlayer = new();

        private Coroutine _musicRoutine = null;
        private AudioAccess _currentTrackAccess = null;

        public string ID => "Music settings";
        public float PlaybackInterval { get; private set; }
        public bool IsPlaying => _musicRoutine != null;

        public MusicPlayer(AudioCollection music)
        {
            if (music == null) throw new ArgumentNullException(nameof(music));
            _music = music;
        }

        public void Play()
        {
            if (IsPlaying == false) _musicRoutine = CoroutineRunner.RunRoutine(PlayMusicForever());
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
                _currentTrackAccess = _music.PlayNonRepeatingRandomAudioClip(Vector2.zero);

                yield return new WaitForSeconds(PlaybackInterval + _currentTrackAccess.PlaybackDuration);
            }
        }

        public void SetPlaybackInterval(float value, bool save)
        {
            PlaybackInterval = Mathf.Clamp(value, MinPlaybackInterval, MaxPlaybackInterval);

            if (save == true) SavingRequested?.Invoke(this, EventArgs.Empty);
        }

        public void SetDefaultPlaybackInterval(bool save) => SetPlaybackInterval(DefaultPlaybackInterval, save);

        #region interfaces

        public void OnInitialize()
        {
            GameServices.Register(this);
        }

        public void OnSubscribe()
        {
            if (GameServices.TryGetService(out SavingSystem system) == true) system.Register(this);
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(SavingSystem));
        }

        public void OnUnsubscribe()
        {
            if (GameServices.TryGetService(out SavingSystem system) == true) system.Deregister(this);
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(SavingSystem));
        }

        public void OnClear()
        {
            GameServices.Deregister(this);
        }

        public void OnRun()
        {
            Play();
        }

        public string GetState() => JsonConvert.SerializeObject(PlaybackInterval);

        public void SetState(string state)
        {
            var playbackInterval = JsonConvert.DeserializeObject<float>(state);
            SetPlaybackInterval(playbackInterval, false);
        }

        public override bool Equals(object obj) => Equals(obj as ISavable);

        public bool Equals(ISavable other) => other is not null && other.ID.Equals(ID);

        public override int GetHashCode() => ID.GetHashCode();

        #endregion
    }
}
