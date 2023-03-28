using System;
using System.Collections.Generic;

namespace SpaceAce.Architecture
{
    public static class GameServices
    {
        private static readonly Dictionary<Type, object> s_services = new();

        public static void Register(object service)
        {
            if (service is null)
            {
                throw new EmptyGameServiceRegistrationAttemptException();
            }

            if (s_services.ContainsKey(service.GetType()))
            {
                throw new DuplicateGameServiceRegistrationAttemptException(service);
            }

            s_services.Add(service.GetType(), service);
        }

        public static void Deregister(object service)
        {
            if (service is null)
            {
                throw new EmptyGameServiceDeregistrationAttemptException();
            }

            if (s_services.ContainsKey(service.GetType()) == false)
            {
                throw new UnregisteredGameServiceDeregistrationAttemptException(service);
            }

            s_services.Remove(service.GetType());
        }

        public static bool TryGetService<T>(out T service)
        {
            if (s_services.TryGetValue(typeof(T), out var value) == true)
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