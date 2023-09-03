using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SpaceAce.Main;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace SpaceAce.Gameplay.Inventories
{
    public sealed class WeaponCoolant : InventoryItem, IEquatable<WeaponCoolant>
    {
        public const float MinCooldownReduction = 0f;
        public const float MaxCooldownReduction = 1f;

        public const float MinFireRateBoost = 0f;
        public const float MaxFirerateBoost = 1f;

        [JsonIgnore]
        public override float Worth => (base.Worth +
                                        CooldownReduction * CooldownReductionUnitWorth +
                                        FireRateBoost * FireRateBoostUnitWorth) *
                                        (float)(Rarity + 1);

        public float CooldownReduction { get; }

        [JsonIgnore]
        public float CooldownReductionPercentage => CooldownReduction * 100f;

        public float FireRateBoost { get; }

        [JsonIgnore]
        public float FireRateBoostPercentage => FireRateBoost * 100f;

        public WeaponCoolant(ItemRarity rarity, float duration, float cooldownReduction, float fireRateBoost) : base(rarity, duration)
        {
            CooldownReduction = Mathf.Clamp(cooldownReduction, MinCooldownReduction, MaxCooldownReduction);
            FireRateBoost = Mathf.Clamp(fireRateBoost, MinFireRateBoost, MaxFirerateBoost);
        }

        public override bool Fuse(InventoryItem item1, InventoryItem item2, out InventoryItem result)
        {
            if (item1 is not null &&
                item2 is not null &&
                Rarity.Equals(item1.Rarity) &&
                Rarity.Equals(item2.Rarity) &&
                item1 is WeaponCoolant other1 &&
                item2 is WeaponCoolant other2)
            {
                ItemRarity nextRarity = GetNextRarity(Rarity);
                float newDuration = (Duration + other1.Duration + other2.Duration) * FusedPropertyFactor;
                float newCooldownReduction = (CooldownReduction + other1.CooldownReduction + other2.CooldownReduction) * FusedPropertyFactor;
                float newFireRateBoost = (FireRateBoost + other1.FireRateBoost + other2.FireRateBoost) * FusedPropertyFactor;

                result = new WeaponCoolant(nextRarity, newDuration, newCooldownReduction, newFireRateBoost);
                return true;
            }

            result = null;
            return false;
        }

        public override bool Use()
        {
            if (GameModeLoader.Access.GameMode == GameMode.Level &&
                SpecialEffectsMediator.Access.TryGetEffectReceivers(out IEnumerable<IWeaponCoolantUser> users) == true)
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
            LocalizedString title = new("Weapon coolant", "Title");
            LocalizedString rarity = new("Rarity", Rarity.ToString());
            LocalizedString stats = new("Weapon coolant", "Stats") { Arguments = new[] { this } };
            LocalizedString description = new("Weapon coolant", "Description");

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

        public override bool Equals(object obj) => Equals(obj as WeaponCoolant);

        public bool Equals(WeaponCoolant other) => base.Equals(other) &&
                                                   CooldownReduction.Equals(other.CooldownReduction) &&
                                                   FireRateBoost.Equals(other.FireRateBoost);

        public override int GetHashCode() => base.GetHashCode() ^ CooldownReduction.GetHashCode() ^ FireRateBoost.GetHashCode();
    }
}