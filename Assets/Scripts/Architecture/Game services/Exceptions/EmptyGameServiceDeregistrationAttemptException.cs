using System;

namespace SpaceAce
{
    namespace Architecture
    {
        public sealed class EmptyGameServiceDeregistrationAttemptException : Exception
        {
            private const string ErrorMessage = "Attempted to deregister an empty game service!";

            public EmptyGameServiceDeregistrationAttemptException() : base(ErrorMessage) { }
        }
    }
}