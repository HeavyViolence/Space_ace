using System;
using UnityEngine;

namespace SpaceAce.Main.Audio
{
    [Serializable]
    public sealed class MusicPlayerSettings
    {
        [SerializeField] private float _playbackInterval;

        public float PlaybackInterval => _playbackInterval;

        public MusicPlayerSettings(float playbackInterval)
        {
            _playbackInterval = playbackInterval;
        }
    }
}