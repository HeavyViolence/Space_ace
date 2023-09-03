using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SpaceAce.Main;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace SpaceAce.Gameplay.Inventories
{
    public sealed class WeaponAccelerant : InventoryItem, IEquatable<WeaponAccelerant>
    {
        public const float MinAmmoSpeedBoost = 0f;
        public const float MaxAmmoSpeedBoost = 1f;

        public const float MinDamageBoost = 0f;
        public const float MaxDamageBoost = 1f;

        public const float MinCooldownIncrease = 0f;
        public const float MaxCooldownIncrease = 1f;

        public float AmmoSpeedBoost { get; }

        [JsonIgnore]
        public float AmmoSpeedBoostPercentage => AmmoSpeedBoost * 100f;

        public float DamageBoost { get; }

        [JsonIgnore]
        public float DamageBoostPercentage => DamageBoost * 100f;

        public float CooldownIncrease { get; }

        [JsonIgnore]
        public float CooldownIncreasePercentage => CooldownIncrease * 100f;

        [JsonIgnore]
        public override float Worth => (base.Worth +
                                        AmmoSpeedBoost * AmmoSpeedBoostUnitWorth +
                                        DamageBoost * DamageBoostUnitWorth +
                                        CooldownIncrease * CooldownIncreaseUnitWorth) *
                                       (float)(Rarity + 1);

        public WeaponAccelerant(ItemRarity rarity,
                                float duration,
                                float ammoSpeedBoost,
                                float damageBoost,
                                float cooldownIncrease) : base(rarity, duration)
        {
            AmmoSpeedBoost = Mathf.Clamp(ammoSpeedBoost, MinAmmoSpeedBoost, MaxAmmoSpeedBoost);
            DamageBoost = Mathf.Clamp(damageBoost, MinDamageBoost, MaxDamageBoost);
            CooldownIncrease = Mathf.Clamp(cooldownIncrease, MinCooldownIncrease, MaxCooldownIncrease);
        }

        public override bool Fuse(InventoryItem item1, InventoryItem item2, out InventoryItem result)
        {
            if (item1 is not null &&
                item2 is not null &&
                Rarity.Equals(item1.Rarity) &&
                Rarity.Equals(item2.Rarity) &&
                item1 is WeaponAccelerant other1 &&
                item2 is WeaponAccelerant other2)
            {
                ItemRarity nextRarity = GetNextRarity(Rarity);
                float newDuration = (Duration + other1.Duration + other2.Duration) * FusedPropertyFactor;
                float newAmmoSpeedBoost = (AmmoSpeedBoost + other1.AmmoSpeedBoost + other2.AmmoSpeedBoost) * FusedPropertyFactor;
                float newdamageBoost = (DamageBoost +  other1.DamageBoost + other2.DamageBoost) * FusedPropertyFactor;
                float cooldownIncrease = (CooldownIncrease + other1.CooldownIncrease + other2.CooldownIncrease) * FusedPropertyFactor;

                result = new WeaponAccelerant(nextRarity, newDuration, newAmmoSpeedBoost, newdamageBoost, cooldownIncrease);
                return true;
            }

            result = null;
            return false;
        }

        public override bool Use()
        {
            if (GameModeLoader.Access.GameMode == GameMode.Level &&
                SpecialEffectsMediator.Access.TryGetEffectReceivers(out IEnumerable<IWeaponAccelerantUser> users) == true)
            {
                bool used = false;

                foreach (var user in users) if (user.Use(this) == true) used = true;

                if (used)
                {
                    HUDDisplay.Access.RegisterActiveItem(this);
                    return true;
                }
            }

            return false;
        }

        public override async UniTask<string> GetDescription()
        {
            LocalizedString title = new("Weapon accelerant", "Title");
            LocalizedString rarity = new("Rarity", Rarity.ToString());
            LocalizedString stats = new("Weapon accelerant", "Stats") { Arguments = new[] { this } };
            LocalizedString description = new("Weapon accelerant", "Description");

            var titleOperation = title.GetLocalizedStringAsync();
            await titleOperation;
            string localizedTitle = titleOperation.Result;

            var rarityOperation = rarity.GetLocalizedStringAsync();
            await rarityOperation;
            string localizedRarity = rarityOperation.Result;

            var statsOperation = stats.GetLocalizedStringAsync();
            await statsOperation;
            string localizedStats = statsOperation.Result;

            var descriptionOperation = description.GetLocalizedStringAsync();
            await descriptionOperation;
            string localizedDescription = descriptionOperation.Result;

            return $"{localizedTitle}\n{localizedRarity}\n\n{localizedStats}\n\n{localizedDescription}";
        }

        public override bool Equals(object obj) => Equals(obj as WeaponAccelerant);

        public bool Equals(WeaponAccelerant other) => base.Equals(other) &&
                                                      AmmoSpeedBoost.Equals(other.AmmoSpeedBoost) &&
                                                      DamageBoost.Equals(other.DamageBoost) &&
                                                      CooldownIncrease.Equals(other.CooldownIncrease);

        public override int GetHashCode() => base.GetHashCode() ^
                                             AmmoSpeedBoost.GetHashCode() ^
                                             DamageBoost.GetHashCode() ^
                                             CooldownIncrease.GetHashCode();
    }
}