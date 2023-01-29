using System;
using System.Collections.Generic;

namespace SpaceAce
{
    namespace Architecture
    {
        public static class GameServices
        {
            private static readonly Dictionary<Type, object> _services = new();

            public static void Register(object service)
            {
                if (service is null)
                {
                    throw new EmptyGameServiceRegistrationAttemptException();
                }

                if (_services.ContainsKey(service.GetType()))
                {
                    throw new DuplicateGameServiceRegistrationAttemptException(service);
                }

                _services.Add(service.GetType(), service);
            }

            public static void Deregister(object service)
            {
                if (service is null)
                {
                    throw new EmptyGameServiceDeregistrationAttemptException();
                }

                if (_services.ContainsKey(service.GetType()) == false)
                {
                    throw new UnregisteredGameServiceDeregistrationAttemptException(service);
                }
                
                _services.Remove(service.GetType());
            }

            public static bool TryGetService<T>(out T service)
            {
                if (_services.TryGetValue(typeof(T), out var value))
                {
                    service = (T)value;

                    return true;
                }
                else
                {
                    service = default;

                    return false;
                }
            }
        }
    }
}