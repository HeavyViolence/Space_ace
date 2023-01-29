using System;

namespace SpaceAce
{
    namespace Architecture
    {
        public sealed class EmptyGameServiceRegistrationAttemptException : Exception
        {
            private const string ErrorMessage = "Attempted to register an empty game service!";

            public EmptyGameServiceRegistrationAttemptException() : base(ErrorMessage) { }
        }
    }
}