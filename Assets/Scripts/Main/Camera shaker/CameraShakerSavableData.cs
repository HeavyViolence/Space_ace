namespace SpaceAce.Main
{
    public sealed class CameraShakerSavableData
    {
        public bool ShakingEnabled { get; private set; }

        public CameraShakerSavableData(bool shakingEnabled)
        {
            ShakingEnabled = shakingEnabled;
        }
    }
}