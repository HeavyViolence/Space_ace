namespace SpaceAce.Architecture
{
    public interface IPausable
    {
        bool Paused { get; }

        void Pause();
        void Resume();
    }
}