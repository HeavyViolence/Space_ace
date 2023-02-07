using System;

namespace SpaceAce.Main.ObjectPooling
{
    public sealed class MissingObjectPoolException : Exception
    {
        private const string ErrorMessage = "Attempted to access a missing object pool. It must be created first!";

        public string AnchorName { get; }

        public MissingObjectPoolException(string anchorName) : base(ErrorMessage)
        {
            AnchorName = anchorName;
        }
    }
}