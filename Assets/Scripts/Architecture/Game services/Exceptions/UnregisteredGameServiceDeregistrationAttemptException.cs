using System;

namespace SpaceAce
{
    namespace Architecture
    {
        public sealed class UnregisteredGameServiceDeregistrationAttemptException : Exception
        {
            private const string ErrorMessage = "Attempted to deregister an unregistered game service!";

            public object Service { get; }
            public Type ServiceType => Service.GetType();

            public UnregisteredGameServiceDeregistrationAttemptException(object service) : base(ErrorMessage)
            {
                Service = service;
            }
        }
    }
}