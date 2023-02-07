using System.Runtime.Serialization;

namespace SpaceAce.Main.Saving
{
    [DataContract]
    public sealed class AudioPlayerSavableData
    {
        [DataMember]
        public float MasterVolume { get; private set; }

        [DataMember]
        public float MusicVolume { get; private set; }

        [DataMember]
        public float ShootingVolume { get; private set; }

        [DataMember]
        public float ExplosionsVolume { get; private set; }

        [DataMember]
        public float InterfaceVolume { get; private set; }

        [DataMember]
        public float BackgroundVolume { get; private set; }

        [DataMember]
        public float NotificationsVolume { get; private set; }

        [DataMember]
        public float InteractionsVolume { get; private set; }

        public AudioPlayerSavableData(float masterVolume,
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