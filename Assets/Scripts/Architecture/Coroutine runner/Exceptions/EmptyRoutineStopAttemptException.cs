using System;

namespace SpaceAce
{
    namespace Architecture
    {
        public sealed class EmptyRoutineStopAttemptException : Exception
        {
            private const string ErrorMessage = "Attempted to stop an empty routine!";

            public EmptyRoutineStopAttemptException() : base(ErrorMessage) { }
        }
    }
}