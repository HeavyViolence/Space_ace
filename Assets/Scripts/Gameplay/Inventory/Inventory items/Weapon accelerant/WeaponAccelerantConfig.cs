using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    [CreateAssetMenu(fileName = "Weapon accelerant", menuName = "Space ace/Configs/Loot/Weapon accelerant")]
    public sealed class WeaponAccelerantConfig : InventoryItemConfig
    {
        [SerializeField] private float _ammoSpeedBoost = WeaponAccelerant.MinAmmoSpeedBoost;
        [SerializeField] private float _ammoSpeedBoostRandomDeviation = 0f;

        [SerializeField] private float _damageBoost = WeaponAccelerant.MinDamageBoost;
        [SerializeField] private float _damageBoostRandomDeviation = 0f;

        [SerializeField] private float _cooldownIncrease = WeaponAccelerant.MinCooldownIncrease;
        [SerializeField] private float _cooldownIncreaseRandomDeviation = 0f;

        public RangedFloat AmmoSpeedBoost { get; private set; }

        public RangedFloat DamageBoost { get; private set; }

        public RangedFloat CooldownIncrease { get; private set; }

        public override void ApplySettings()
        {
            base.ApplySettings();

            AmmoSpeedBoost = new(_ammoSpeedBoost,
                                 _ammoSpeedBoostRandomDeviation,
                                 WeaponAccelerant.MinAmmoSpeedBoost,
                                 WeaponAccelerant.MaxAmmoSpeedBoost);

            DamageBoost = new(_damageBoost,
                              _damageBoostRandomDeviation,
                              WeaponAccelerant.MinDamageBoost,
                              WeaponAccelerant.MaxDamageBoost);

            CooldownIncrease = new(_cooldownIncrease,
                                   _cooldownIncreaseRandomDeviation,
                                   WeaponAccelerant.MinCooldownIncrease,
                                   WeaponAccelerant.MaxCooldownIncrease);
        }

        public override InventoryItem GetItem() => new WeaponAccelerant(Rarity,
                                                                        Duration.RandomValue,
                                                                        AmmoSpeedBoost.RandomValue,
                                                                        DamageBoost.RandomValue,
                                                                        CooldownIncrease.RandomValue);
    }
}