namespace SpaceAce.Main.Audio
{
    public sealed class AudioPlayerSettings
    {
        public static AudioPlayerSettings Default => new(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);

        public float MasterVolume { get; }
        public float MusicVolume { get; }
        public float ShootingVolume { get; }
        public float ExplosionsVolume { get; }
        public float InterfaceVolume { get; }
        public float BackgroundVolume { get; }
        public float NotificationsVolume { get; }
        public float InteractionsVolume { get; }

        public AudioPlayerSettings(float masterVolume,
                                   float musicVolume,
                                   float shootingVolume,
                                   float explosionsVolume,
                                   float interfaceVolume,
                                   float backgroundVolume,
                                   float notificationsVolume,
                                   float interactionsVolume)
        {
            MasterVolume = masterVolume;
            MusicVolume = musicVolume;
            ShootingVolume = shootingVolume;
            ExplosionsVolume = explosionsVolume;
            InterfaceVolume = interfaceVolume;
            BackgroundVolume = backgroundVolume;
            NotificationsVolume = notificationsVolume;
            InteractionsVolume = interactionsVolume;
        }
    }
}