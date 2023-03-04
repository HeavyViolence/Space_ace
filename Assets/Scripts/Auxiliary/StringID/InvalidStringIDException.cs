using System;

namespace SpaceAce.Auxiliary
{
    public sealed class InvalidStringIDException : Exception
    {
        private const string ErrorMessage = "A valid string ID has the following structure: ####-####-####-####. " +
                                            "# is being substituted for an upper case or lower case english letter, or a digit.";

        public InvalidStringIDException() : base(ErrorMessage) { }
    }
}