using System;

namespace SpaceAce
{
    namespace Architecture
    {
        public sealed class UnregisteredGameServiceAccessAttemptException : Exception
        {
            private const string ErrorMessage = "Attempted to retrieve an unregistered game service!";

            public Type ServiceType { get; }

            public UnregisteredGameServiceAccessAttemptException(Type serviceType) : base(ErrorMessage)
            {
                ServiceType = serviceType;
            }
        }
    }
}