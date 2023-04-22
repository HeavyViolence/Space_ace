using SpaceAce.Architecture;
using SpaceAce.Auxiliary;
using SpaceAce.Main.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SpaceAce.Main.Audio
{
    public sealed class AudioPlayer : IInitializable, ISavable
    {
        private const int MaxAudioSources = 32;

        public event EventHandler SavingRequested;

        private const float MinVolume = -80f;
        private const float MaxVolume = 0f;

        private readonly Dictionary<string, AudioSource> _activeAudioSources = new(MaxAudioSources);
        private readonly Stack<AudioSource> _availableAudioSources = new(MaxAudioSources);
        private readonly AudioMixer _audioMixer;
        private Transform _audioSourcePoolAnchor;

        public string ID { get; }
        public string SaveName => "Audio player settings";
        public AudioPlayerSettings Settings { get; private set; }

        public AudioPlayer(string id, AudioMixer audioMixer)
        {
            if (StringID.IsValid(id) == false)
            {
                throw new InvalidStringIDException();
            }

            ID = id;

            if (audioMixer == null)
            {
                throw new ArgumentNullException(nameof(audioMixer), $"Attempted to set an empty {nameof(AudioMixer)}!");
            }

            _audioMixer = audioMixer;
            Settings = AudioPlayerSettings.Default;
            CreateAudioSourcePool();
        }

        public void ApplySettings(AudioPlayerSettings settings, bool save)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings), $"Attempted to apply an empty {nameof(AudioPlayerSettings)}!");
            }

            SetVolume("Master volume", settings.MasterVolume);
            SetVolume("Shooting volume", settings.ShootingVolume);
            SetVolume("Explosions volume", settings.ExplosionsVolume);
            SetVolume("Background volume", settings.BackgroundVolume);
            SetVolume("Interface volume", settings.InterfaceVolume);
            SetVolume("Music volume", settings.MusicVolume);
            SetVolume("Interactions volume", settings.InteractionsVolume);
            SetVolume("Notifications volume", settings.NotificationsVolume);

            Settings = settings;

            if (save == true)
            {
                SavingRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        private void SetVolume(string name, float volume)
        {
            float clampedVolume = Mathf.Clamp(volume, MinVolume, MaxVolume);
            _audioMixer.SetFloat(name, clampedVolume);
        }

        public AudioAccess Play(AudioProperties properties) => ConfigureAudioSource(FindAvailableAudioSource(), properties);

        public bool InterruptPlay(string id) => DisableActiveAudioSource(id);

        private void CreateAudioSourcePool()
        {
            _audioSourcePoolAnchor = new GameObject("Audio player audio source pool").transform;

            for (int i = 0; i < MaxAudioSources; i++)
            {
                var audioSourceHolder = new GameObject($"Audio source #{i + 1}");
                audioSourceHolder.transform.parent = _audioSourcePoolAnchor;
                var audioSource = audioSourceHolder.AddComponent<AudioSource>();

                SetAudioSourceDefaultState(audioSource);
                _availableAudioSources.Push(audioSource);
            }
        }

        private bool DisableActiveAudioSource(string id)
        {
            if (_activeAudioSources.TryGetValue(id, out AudioSource source) == true)
            {
                SetAudioSourceDefaultState(source);
                _activeAudioSources.Remove(id);
                _availableAudioSources.Push(source);

                return true;
            }
            else
            {
                return false;
            }
        }

        private void SetAudioSourceDefaultState(AudioSource source)
        {
            source.Stop();

            source.clip = null;
            source.outputAudioMixerGroup = null;
            source.mute = true;
            source.bypassEffects = true;
            source.bypassListenerEffects = true;
            source.bypassReverbZones = true;
            source.playOnAwake = false;
            source.loop = false;
            source.priority = byte.MaxValue;
            source.volume = 0f;
            source.spatialBlend = 0f;
            source.pitch = 1f;
            source.reverbZoneMix = 0f;

            source.transform.parent = _audioSourcePoolAnchor;
            source.transform.position = Vector3.zero;
            source.gameObject.SetActive(false);
        }

        private AudioAccess ConfigureAudioSource(AudioSource source, AudioProperties properties)
        {
            var id = StringID.NextCryptosafe();
            AudioAccess access;

            source.clip = properties.Clip;
            source.outputAudioMixerGroup = properties.OutputAudioGroup;
            source.mute = false;
            source.bypassEffects = false;
            source.bypassListenerEffects = false;
            source.bypassReverbZones = false;
            source.priority = (byte)properties.Priority;
            source.volume = properties.Volume;
            source.spatialBlend = properties.SpatialBlend;
            source.pitch = properties.Pitch;

            if (properties.Loop == true)
            {
                source.loop = true;
                source.transform.parent = properties.AudioSourceAnchor;
                source.transform.localPosition = Vector3.zero;

                access = new AudioAccess(id, float.PositiveInfinity);
            }
            else
            {
                source.loop = false;
                source.transform.position = properties.PlayPosition;

                access = new AudioAccess(id, properties.Clip.length / properties.Pitch);
                CoroutineRunner.RunRoutine(WaitToDisableActiveAudioSource(access));
            }
            
            source.gameObject.SetActive(true);
            source.Play();

            _activeAudioSources.Add(id, source);
            
            return access;
        }

        private IEnumerator WaitToDisableActiveAudioSource(AudioAccess access)
        {
            yield return new WaitForSeconds(access.PlaybackDuration);

            DisableActiveAudioSource(access.ID);
        }

        private AudioSource FindAvailableAudioSource()
        {
            if (_availableAudioSources.Count > 0)
            {
                return _availableAudioSources.Pop();
            }

            byte priority = 0;
            string id = string.Empty;
            AudioSource availableSource = null;

            foreach (var source in _activeAudioSources)
            {
                if (source.Value.loop == true)
                {
                    continue;
                }

                if (source.Value.priority > priority)
                {
                    priority = (byte)source.Value.priority;
                    id = source.Key;
                    availableSource = source.Value;
                }
            }

            _activeAudioSources.Remove(id);
            SetAudioSourceDefaultState(availableSource);

            return availableSource;
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
        }

        public void OnClear()
        {
            GameServices.Deregister(this);
        }

        public string GetState() => JsonUtility.ToJson(Settings);

        public void SetState(string state)
        {
            var settings = JsonUtility.FromJson<AudioPlayerSettings>(state);

            ApplySettings(settings, false);
        }

        public override bool Equals(object obj) => Equals(obj as ISavable);

        public bool Equals(ISavable other) => other is not null && other.ID.Equals(ID);

        public override int GetHashCode() => ID.GetHashCode();

        #endregion
    }
}