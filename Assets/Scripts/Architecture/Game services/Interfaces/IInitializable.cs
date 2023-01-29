namespace SpaceAce
{
    namespace Architecture
    {
        public interface IInitializable
        {
            public void OnInitialize();
            public void OnSubscribe();
            public void OnUnsubscribe();
            public void OnClear();
        }
    }
}