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
        public const float MinPitch = 0.5f;
        public const float MaxPitch = 2f;
        public const float DefaultPitch = 1f;

        private int _nextAudioClipIndex = 0;
        private static GameServiceFastAccess<AudioPlayer> s_audioPlayer = new();

        [SerializeField] private List<AudioClip> _audioClips;

        [SerializeField] private AudioMixerGroup _outputAudioGroup;

        [SerializeField] private float _volume = 1f;
        [SerializeField] private float _volumeDeviation = 0f;

        [SerializeField] private AudioClipPriority _priority = AudioClipPriority.Lowest;

        [SerializeField] private float _spatialBlend = 0.5f;
        [SerializeField] private float _spatialBlendDeviation = 0f;

        [SerializeField] private float _pitch = DefaultPitch;
        [SerializeField] private float _pitchDeviation = 0f;

        public AudioMixerGroup OutputAudioGroup => _outputAudioGroup;
        public float Volume => _volume + _volumeDeviation * AuxMath.RandomNormal;
        public AudioClipPriority Priority => _priority;
        public float SpatialBlend => _spatialBlend + _spatialBlendDeviation * AuxMath.RandomNormal;
        public float Pitch => _pitch + _pitchDeviation * AuxMath.RandomNormal;
        public AudioClip RandomAudioClip => _audioClips[Random.Range(0, _audioClips.Count)];
        public AudioClip NextAudioClip => _audioClips[_nextAudioClipIndex++ % _audioClips.Count];
        public int ClipsAmount => _audioClips.Count;

        public AudioAccess PlayRandomAudioClip(Vector2 position) => PlayAudioClip(RandomAudioClip, position);

        public AudioAccess PlayNextAudioClip(Vector2 position) => PlayAudioClip(NextAudioClip, position);

        public AudioAccess PlayRandomAudioClipOnLoop(Transform parent) => PlayAudioClipOnLoop(RandomAudioClip, parent);

        public AudioAccess PlayNextAudioClipOnLoop(Transform parent) => PlayAudioClipOnLoop(NextAudioClip, parent);

        private AudioAccess PlayAudioClip(AudioClip clip, Vector2 position)
        {
            AudioProperties properties = new(clip,
                                             OutputAudioGroup,
                                             Volume,
                                             Priority,
                                             SpatialBlend,
                                             Pitch,
                                             false,
                                             null,
                                             position);

            return s_audioPlayer.Access.Play(properties);
        }

        private AudioAccess PlayAudioClipOnLoop(AudioClip clip, Transform parent)
        {
            AudioProperties properties = new(clip,
                                             OutputAudioGroup,
                                             Volume,
                                             Priority,
                                             SpatialBlend,
                                             Pitch,
                                             true,
                                             parent,
                                             Vector3.zero);

            return s_audioPlayer.Access.Play(properties);
        }
    }
}