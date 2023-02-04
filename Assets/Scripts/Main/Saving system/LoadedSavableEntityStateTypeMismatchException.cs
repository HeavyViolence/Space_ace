using System;

namespace SpaceAce.Main.Saving
{
    public sealed class LoadedSavableEntityStateTypeMismatchException : Exception
    {
        private const string ErrorMessage = "Savable entity must retrieve a state of the same type is has given upon save!";

        public Type EncounteredType { get; }
        public Type ExpectedType { get; }
        public Type SavableEntityType { get; }

        public LoadedSavableEntityStateTypeMismatchException(Type encounteredType,
                                                             Type expectedType,
                                                             Type savableEntityType) : base(ErrorMessage)
        {
            EncounteredType = encounteredType;
            ExpectedType = expectedType;
            SavableEntityType = savableEntityType;
        }
    }
}