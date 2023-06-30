namespace SpaceAce.UI
{
    public interface IHealthView
    {
        float MaxValue { get; }
        float Value { get; }
        float ValuePercentage { get; }
        float RegenPerSecond { get; }
    }
}