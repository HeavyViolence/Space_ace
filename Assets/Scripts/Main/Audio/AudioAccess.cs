using System;

namespace SpaceAce.Main.Audio
{
    public sealed class AudioAccess : IEquatable<AudioAccess>
    {
        public string ID { get; }
        public float PlaybackDuration { get; }

        public AudioAccess(string id, float playbackDuration)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id), "Attempted to assign an invalid ID!");
            }

            ID = id;

            if (playbackDuration <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(playbackDuration), $"Playback duration must be positive!");
            }

            PlaybackDuration = playbackDuration;
        }

        public override bool Equals(object obj) => Equals(obj as AudioAccess);

        public bool Equals(AudioAccess other) => other is not null && other.ID.Equals(ID);

        public static bool operator ==(AudioAccess x, AudioAccess y)
        {
            if (x is null)
            {
                if (y is null)
                {
                    return true;
                }

                return false;
            }

            return x.Equals(y);
        }

        public static bool operator !=(AudioAccess x, AudioAccess y) => !(x == y);

        public override int GetHashCode() => ID.GetHashCode();

        public override string ToString() => $"{nameof(AudioAccess)} of: ID = {ID}, playback duration = {PlaybackDuration}";
    }
}