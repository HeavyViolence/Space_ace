using SpaceAce.Auxiliary;
using SpaceAce.Gameplay.Inventories;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CreateAssetMenu(fileName = "Reactive armor", menuName = "Space ace/Configs/Loot/Reactive armor")]
    public sealed class ReactiveArmorConfig : InventoryItemConfig
    {
        [SerializeField] private float _movementSlowdown = ReactiveArmor.MinMovementSlowdown;
        [SerializeField] private float _movementSlowdownRandomDeviation = 0f;

        [SerializeField] private float _healthIncrease = ReactiveArmor.MinHealthIncrease;
        [SerializeField] private float _healthIncreaseRandomDeviation = 0f;

        [SerializeField] private float _damageToArmorConversionRate = ReactiveArmor.MinDamageToArmorConversionRate;
        [SerializeField] private float _damageToArmorConversionRateRandomDeviation = 0f;

        public RangedFloat MovementSlowdown { get; private set; }

        public RangedFloat HealthIncrease { get; private set; }

        public RangedFloat DamageToArmorConversionRate { get; private set; }

        public override void ApplySettings()
        {
            base.ApplySettings();

            MovementSlowdown = new(_movementSlowdown, _movementSlowdownRandomDeviation, ReactiveArmor.MinMovementSlowdown, ReactiveArmor.MaxMovementSlowdown);
            HealthIncrease = new(_healthIncrease, _healthIncreaseRandomDeviation, ReactiveArmor.MinHealthIncrease, ReactiveArmor.MaxHealthIncrease);
            DamageToArmorConversionRate = new(_damageToArmorConversionRate,
                                              _damageToArmorConversionRateRandomDeviation,
                                              ReactiveArmor.MinDamageToArmorConversionRate,
                                              ReactiveArmor.MaxDamageToArmorConversionRate);
        }

        public override InventoryItem GetItem() => new ReactiveArmor(Rarity,
                                                                     Duration.RandomValue,
                                                                     MovementSlowdown.RandomValue,
                                                                     HealthIncrease.RandomValue,
                                                                     DamageToArmorConversionRate.RandomValue);
    }
}