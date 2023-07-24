namespace SpaceAce.Main.Audio
{
    public sealed class AudioPlayerSettings
    {
        public static AudioPlayerSettings Default => new(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);

        public float MasterVolume { get; private set; }
        public float MusicVolume { get; private set; }
        public float ShootingVolume { get; private set; }
        public float ExplosionsVolume { get; private set; }
        public float InterfaceVolume { get; private set; }
        public float BackgroundVolume { get; private set; }
        public float NotificationsVolume { get; private set; }
        public float InteractionsVolume { get; private set; }

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