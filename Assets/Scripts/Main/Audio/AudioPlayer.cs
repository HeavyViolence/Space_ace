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
        private Transform _audioSourcePoolAnchor;

        private bool _suppressSaveRequest = false;
        private readonly AudioMixer _audioMixer;

        public string ID { get; }
        public string SaveName => "Audio player settings";
        public float MasterVolume { get; private set; }
        public float ShootingVolume { get; private set; }
        public float ExplosionsVolume { get; private set; }
        public float InteractionsVolume { get; private set; }
        public float MusicVolume { get; private set; }
        public float BackgroundVolume { get; private set; }
        public float InterfaceVolume { get; private set; }
        public float NotificationsVolume { get; private set; }

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

            CreateAudioSourcePool();
        }

        public void SetMasterVolume(float volume) => SetVolume("Master volume", volume);

        public void SetShootingVolume(float volume) => SetVolume("Shooting volume", volume);

        public void SetExplosionsVolume(float volume) => SetVolume("Explosions volume", volume);

        public void SetBackgroundVolume(float volume) => SetVolume("Background volume", volume);

        public void SetInterfaceVolume(float volume) => SetVolume("Interface volume", volume);

        public void SetMusicVolume(float volume) => SetVolume("Music volume", volume);

        public void SetInteractionsVolume(float volume) => SetVolume("Interactions volume", volume);

        public void SetNotificationsVolume(float volume) => SetVolume("Notifications volume", volume);

        private void SetVolume(string name, float volume)
        {
            float clampedVolume = Mathf.Clamp(volume, MinVolume, MaxVolume);
            _audioMixer.SetFloat(name, clampedVolume);

            switch (name)
            {
                case "Master volume":
                    {
                        MasterVolume = volume;
                        break;
                    }
                case "Shooting volume":
                    {
                        ShootingVolume = volume;
                        break;
                    }
                case "Explosions volume":
                    {
                        ExplosionsVolume = volume;
                        break;
                    }
                case "Background volume":
                    {
                        BackgroundVolume = volume;
                        break;
                    }
                case "Interface volume":
                    {
                        InterfaceVolume = volume;
                        break;
                    }
                case "Music volume":
                    {
                        MusicVolume = volume;
                        break;
                    }
                case "Interactions volume":
                    {
                        InteractionsVolume = volume;
                        break;
                    }
                case "Notifications volume":
                    {
                        NotificationsVolume = volume;
                        break;
                    }
            }

            if (_suppressSaveRequest == false) SavingRequested?.Invoke(this, EventArgs.Empty);
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

        public object GetState() => new AudioPlayerSavableData(MasterVolume,
                                                               MusicVolume,
                                                               ShootingVolume,
                                                               ExplosionsVolume,
                                                               InterfaceVolume,
                                                               BackgroundVolume,
                                                               NotificationsVolume,
                                                               InteractionsVolume);

        public void SetState(object state)
        {
            if (state is AudioPlayerSavableData value)
            {
                _suppressSaveRequest = true;

                MasterVolume = value.MasterVolume;
                MusicVolume = value.MusicVolume;
                ShootingVolume = value.ShootingVolume;
                ExplosionsVolume = value.ExplosionsVolume;
                InterfaceVolume = value.InterfaceVolume;
                BackgroundVolume = value.BackgroundVolume;
                NotificationsVolume = value.NotificationsVolume;
                InteractionsVolume = value.InteractionsVolume;

                _suppressSaveRequest = false;
            }
            else
            {
                throw new LoadedSavableEntityStateTypeMismatchException(state.GetType(), typeof(AudioPlayerSavableData), GetType());
            }
        }

        public override bool Equals(object obj) => Equals(obj as ISavable);

        public bool Equals(ISavable other) => other is not null && other.ID.Equals(ID);

        public override int GetHashCode() => ID.GetHashCode();

        #endregion
    }
}