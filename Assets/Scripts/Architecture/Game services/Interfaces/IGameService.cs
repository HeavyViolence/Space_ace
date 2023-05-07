namespace SpaceAce.Architecture
{
    public interface IGameService
    {
        void OnInitialize();
        void OnSubscribe();
        void OnUnsubscribe();
        void OnClear();
    }
}