namespace SpaceAce.UI
{
    public interface IWeaponView
    {
        int ActiveWeaponGroupIndex { get; }
        int WeaponGroupsAmount { get; }

        float MaxDamagePerSecond { get; }
    }
}