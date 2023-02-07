namespace SpaceAce.Architecture
{
    public sealed class GameServiceFastAccess<T>
    {
        private T _access;

        public T Access
        {
            get
            {
                if (_access is null)
                {
                    if (GameServices.TryGetService(out T service) == true)
                    {
                        _access = service;
                    }
                    else
                    {
                        throw new UnregisteredGameServiceAccessAttemptException(typeof(T));
                    }
                }

                return _access;
            }
        }
    }
}