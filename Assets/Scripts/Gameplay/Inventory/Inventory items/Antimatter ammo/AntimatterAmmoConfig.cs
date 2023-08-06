using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    [CreateAssetMenu(fileName = "Antimatter ammo", menuName = "Space ace/Configs/Loot/Antimatter ammo")]
    public sealed class AntimatterAmmoConfig : InventoryItemConfig
    {
        [SerializeField] private float _consecutiveDamageFactor = AntimatterAmmo.MinConsecutiveDamageFactor;
        [SerializeField] private float _consecutiveDamageFactorRandomDeviation = 0f;

        public RangedFloat ConsecutiveDamageFactor { get; private set; }

        public override void ApplySettings()
        {
            base.ApplySettings();

            ConsecutiveDamageFactor = new(_consecutiveDamageFactor,
                                          _consecutiveDamageFactorRandomDeviation,
                                          AntimatterAmmo.MinConsecutiveDamageFactor,
                                          AntimatterAmmo.MaxConsecutiveDamageFactor);
        }

        public override InventoryItem GetItem() => new AntimatterAmmo(Rarity,
                                                                      Duration.RandomValue,
                                                                      ConsecutiveDamageFactor.RandomValue);
    }
}