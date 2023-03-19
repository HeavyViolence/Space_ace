using SpaceAce.Architecture;
using SpaceAce.Auxiliary;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SpaceAce.Main.Audio
{
    [CreateAssetMenu(fileName = "Audio collection", menuName = "Space ace/Configs/Audio collection")]
    public sealed class AudioCollection : ScriptableObject
    {
        public const float MinSpatialBlend = 0f;
        public const float MaxSpatialBlend = 1f;
        public const float DefaultSpatialBlend = 0.5f;

        public const float MinPitch = 0.5f;
        public const float MaxPitch = 2f;
        public const float DefaultPitch = 1f;

        private static GameServiceFastAccess<AudioPlayer> s_audioPlayer = new();

        private int _nextAudioClipIndex = 0;
        private Queue<int> _nonRepeatingAudioClipsIndices;

        [SerializeField] private List<AudioClip> _audioClips;

        [SerializeField] private AudioMixerGroup _outputAudioGroup;

        [SerializeField] private float _volume = 1f;
        [SerializeField] private float _volumeRandomDeviation = 0f;

        [SerializeField] private AudioClipPriority _priority = AudioClipPriority.Lowest;

        [SerializeField] private float _spatialBlend = DefaultSpatialBlend;
        [SerializeField] private float _spatialBlendRandomDeviation = 0f;

        [SerializeField] private float _pitch = DefaultPitch;
        [SerializeField] private float _pitchRandomDeviation = 0f;

        public AudioMixerGroup OutputAudioGroup => _outputAudioGroup;
        public RangedFloat Volume { get; private set; }
        public AudioClipPriority Priority => _priority;
        public RangedFloat SpatialBlend { get; private set; }
        public RangedFloat Pitch { get; private set; }
        public AudioClip RandomAudioClip => _audioClips[Random.Range(0, _audioClips.Count)];
        public AudioClip NonRepeatingRandomAudioClip
        {
            get
            {
                if (_nonRepeatingAudioClipsIndices.Count == 0)
                {
                    ReplenishNonRepeatingAudioClipsIndices();
                }

                int index = _nonRepeatingAudioClipsIndices.Dequeue();

                return _audioClips[index];
            }
        }
        public AudioClip NextAudioClip => _audioClips[_nextAudioClipIndex++ % _audioClips.Count];
        public int AudioClipsAmount => _audioClips.Count;

        public AudioAccess PlayRandomAudioClip(Vector2 position) => PlayAudioClip(RandomAudioClip, position);
        public AudioAccess PlayRandomAudioClipOnLoop(Transform parent) => PlayAudioClipOnLoop(RandomAudioClip, parent);

        public AudioAccess PlayNonRepeatingRandomAudioClip(Vector2 position) => PlayAudioClip(NonRepeatingRandomAudioClip, position);
        public AudioAccess PlayNonRepeatingAudioClipOnLoop(Transform parent) => PlayAudioClipOnLoop(NonRepeatingRandomAudioClip, parent);

        public AudioAccess PlayNextAudioClip(Vector2 position) => PlayAudioClip(NextAudioClip, position);
        public AudioAccess PlayNextAudioClipOnLoop(Transform parent) => PlayAudioClipOnLoop(NextAudioClip, parent);

        private AudioAccess PlayAudioClip(AudioClip clip, Vector2 position)
        {
            AudioProperties properties = new(clip,
                                             OutputAudioGroup,
                                             Volume.RandomValue,
                                             Priority,
                                             SpatialBlend.RandomValue,
                                             Pitch.RandomValue,
                                             false,
                                             null,
                                             position);

            return s_audioPlayer.Access.Play(properties);
        }

        private AudioAccess PlayAudioClipOnLoop(AudioClip clip, Transform parent)
        {
            AudioProperties properties = new(clip,
                                             OutputAudioGroup,
                                             Volume.RandomValue,
                                             Priority,
                                             SpatialBlend.RandomValue,
                                             Pitch.RandomValue,
                                             true,
                                             parent,
                                             Vector3.zero);

            return s_audioPlayer.Access.Play(properties);
        }

        private void ReplenishNonRepeatingAudioClipsIndices()
        {
            var indices = AuxMath.GenerateRandomNumbersWithoutRepetition(0, _audioClips.Count, _audioClips.Count);
            var enumerator = indices.GetEnumerator();

            while (enumerator.MoveNext() == true)
            {
                _nonRepeatingAudioClipsIndices.Enqueue(enumerator.Current);
            }
        }

        private void OnEnable()
        {
            ApplySettings();
        }

        public void ApplySettings()
        {
            if (_audioClips is not null)
            {
                _nonRepeatingAudioClipsIndices = new(_audioClips.Count);
            }

            Volume = new(_volume, _volumeRandomDeviation);
            SpatialBlend = new(_spatialBlend, _spatialBlendRandomDeviation, MinSpatialBlend, MaxSpatialBlend);
            Pitch = new(_pitch, _pitchRandomDeviation, MinPitch, MaxPitch);
        }
    }
}