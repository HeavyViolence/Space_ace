using System;

namespace SpaceAce
{
    namespace Architecture
    {
        public sealed class EmptyRoutineRunAttemptException : Exception
        {
            private const string ErrorMessage = "Attempted to run an empty routine!";

            public EmptyRoutineRunAttemptException() : base(ErrorMessage) { }
        }
    }
}