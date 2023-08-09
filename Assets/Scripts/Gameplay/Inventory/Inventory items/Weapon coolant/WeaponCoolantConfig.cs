using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    [CreateAssetMenu(fileName = "Weapon coolant", menuName = "Space ace/Configs/Loot/Weapon coolant")]
    public sealed class WeaponCoolantConfig : InventoryItemConfig
    {
        [SerializeField] private float _cooldownReduction = WeaponCoolant.MinCooldownReduction;
        [SerializeField] private float _cooldownReductionRandomDeviation = 0f;

        [SerializeField] private float _fireRateBoost = WeaponCoolant.MinFireRateBoost;
        [SerializeField] private float _fireRateBoostRandomDeviation = 0f;

        public RangedFloat CooldownReduction { get; private set; }

        public RangedFloat FireRateBoost { get; private set; }

        public override void ApplySettings()
        {
            base.ApplySettings();

            CooldownReduction = new(_cooldownReduction,
                                    _cooldownReductionRandomDeviation,
                                    WeaponCoolant.MinCooldownReduction,
                                    WeaponCoolant.MaxCooldownReduction);

            FireRateBoost = new(_fireRateBoost,
                                _fireRateBoostRandomDeviation,
                                WeaponCoolant.MinFireRateBoost,
                                WeaponCoolant.MaxFirerateBoost);
        }

        public override InventoryItem GetItem() => new WeaponCoolant(Rarity,
                                                                     Duration.RandomValue,
                                                                     CooldownReduction.RandomValue,
                                                                     FireRateBoost.RandomValue);
    }
}