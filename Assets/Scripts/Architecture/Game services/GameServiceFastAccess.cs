using System;

namespace SpaceAce
{
    namespace Architecture
    {
        public sealed class GameServiceFastAccess<T> where T : Type
        {
            private T _access = null;

            public T Access
            {
                get
                {
                    if (_access is null)
                    {
                        if (GameServices.TryGetService(out T service))
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
}