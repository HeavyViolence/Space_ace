using System;

namespace SpaceAce.Main.Saving
{
    public sealed class EmptySavableStateEntryException : Exception
    {
        public const string ErrorMessage = "An empty savable state has been passed!";

        public Type ExpectedType { get; }

        public EmptySavableStateEntryException(Type expectedType) : base(ErrorMessage)
        {
            ExpectedType = expectedType;
        }

    }
}