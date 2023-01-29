using System;

namespace SpaceAce
{
    namespace Architecture
    {
        public sealed class DuplicateGameServiceRegistrationAttemptException : Exception
        {
            private const string ErrorMessage = "Attempted to register an already registered game service!";

            public object Service { get; }
            public Type ServiceType => Service.GetType();

            public DuplicateGameServiceRegistrationAttemptException(object service) : base(ErrorMessage)
            {
                Service = service;
            }
        }
    }
}