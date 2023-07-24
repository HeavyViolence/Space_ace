namespace SpaceAce.Main.Audio
{
    public sealed class MusicPlayerSettings
    {
        public float PlaybackInterval { get; private set; }

        public MusicPlayerSettings(float playbackInterval)
        {
            PlaybackInterval = playbackInterval;
        }
    }
}