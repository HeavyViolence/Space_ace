using System;

namespace SpaceAce.Auxiliary
{
    public sealed class InvalidStringIDException : Exception
    {
        private const string ErrorMessage = "A valid string ID has to have the following structure: ####-####-####-####!" +
                                            "Here, # must belong to the ASCII range of [48;57] V [65;90] V [97;122].";

        public InvalidStringIDException() : base(ErrorMessage) { }
    }
}