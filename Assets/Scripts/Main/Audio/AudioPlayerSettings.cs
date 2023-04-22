using System;
using UnityEngine;

namespace SpaceAce.Main.Audio
{
    [Serializable]
    public sealed class AudioPlayerSettings
    {
        public static AudioPlayerSettings Default => new(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);

        [SerializeField] private float _masterVolume;
        [SerializeField] private float _musicVolume;
        [SerializeField] private float _shootingVolume;
        [SerializeField] private float _explosionsVolume;
        [SerializeField] private float _interfaceVolume;
        [SerializeField] private float _backgroundVolume;
        [SerializeField] private float _notificationsVolume;
        [SerializeField] private float _interactionsVolume;

        public float MasterVolume => _masterVolume;
        public float MusicVolume => _musicVolume;
        public float ShootingVolume => _shootingVolume;
        public float ExplosionsVolume => _explosionsVolume;
        public float InterfaceVolume => _interfaceVolume;
        public float BackgroundVolume => _backgroundVolume;
        public float NotificationsVolume => _notificationsVolume;
        public float InteractionsVolume => _interactionsVolume;

        public AudioPlayerSettings(float masterVolume,
                                   float musicVolume,
                                   float shootingVolume,
                                   float explosionsVolume,
                                   float interfaceVolume,
                                   float backgroundVolume,
                                   float notificationsVolume,
                                   float interactionsVolume)
        {
            _masterVolume = masterVolume;
            _musicVolume = musicVolume;
            _shootingVolume = shootingVolume;
            _explosionsVolume = explosionsVolume;
            _interfaceVolume = interfaceVolume;
            _backgroundVolume = backgroundVolume;
            _notificationsVolume = notificationsVolume;
            _interactionsVolume = interactionsVolume;
        }
    }
}