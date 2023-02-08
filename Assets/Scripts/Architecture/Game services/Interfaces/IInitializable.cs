namespace SpaceAce.Architecture
{
    public interface IInitializable
    {
        void OnInitialize();
        void OnSubscribe();
        void OnUnsubscribe();
        void OnClear();
    }
}