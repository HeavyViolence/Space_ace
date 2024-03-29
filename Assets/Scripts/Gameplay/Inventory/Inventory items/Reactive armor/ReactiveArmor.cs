using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SpaceAce.Main;
using System;
using UnityEngine;
using UnityEngine.Localization;

namespace SpaceAce.Gameplay.Inventories
{
    public sealed class ReactiveArmor : InventoryItem, IEquatable<ReactiveArmor>
    {
        public const float MinMovementSlowdown = 0f;
        public const float MaxMovementSlowdown = 0.5f;

        public const float MinHealthIncrease = 500f;
        public const float MaxHealthIncrease = 5000f;

        public const float MinDamageToArmorConversionRate = 0f;
        public const float MaxDamageToArmorConversionRate = 1f;

        [JsonIgnore]
        public override float Worth => (base.Worth +
                                       MovementSlowdown * PlayerSlowdownUnitWorth +
                                       HealthIncrease * HealthUnitWorth +
                                       DamageToArmorConversionRate * ConversionUnitWorth) *
                                       (float)(Rarity + 1);

        public float MovementSlowdown { get; }

        [JsonIgnore]
        public float MovementSlowdownPercentage => MovementSlowdown * 100f;

        public float HealthIncrease { get; }

        public float DamageToArmorConversionRate { get; }

        [JsonIgnore]
        public float DamageToArmorConversionRatePercentage => DamageToArmorConversionRate * 100f;

        public ReactiveArmor(ItemRarity rarity,
                             float duration,
                             float movementSlowdown,
                             float healthIncrease,
                             float damageToArmorConversionRate) : base(rarity, duration)
        {
            MovementSlowdown = Mathf.Clamp(movementSlowdown, MinMovementSlowdown, MaxMovementSlowdown);
            HealthIncrease = Mathf.Clamp(healthIncrease, MinHealthIncrease, MaxHealthIncrease);
            DamageToArmorConversionRate = Mathf.Clamp(damageToArmorConversionRate, MinDamageToArmorConversionRate, MaxDamageToArmorConversionRate);
        }

        public override bool Fuse(InventoryItem item1, InventoryItem item2, out InventoryItem result)
        {
            if (item1 is not null &&
                item2 is not null &&
                Rarity.Equals(item1.Rarity) &&
                Rarity.Equals(item2.Rarity) &&
                item1 is ReactiveArmor other1 &&
                item2 is ReactiveArmor other2)
            {
                ItemRarity nextRarity = GetNextRarity(Rarity);
                float newDuration = (Duration + other1.Duration + other2.Duration) * FusedPropertyFactor;
                float newMovementSlowdown = (MovementSlowdown + other1.MovementSlowdown + other2.MovementSlowdown) * FusedPropertyFactor;
                float newHealthIncrease = (HealthIncrease + other1.HealthIncrease + other2.HealthIncrease) * FusedPropertyFactor;
                float newDamageToArmorConversionRate = (DamageToArmorConversionRate + other1.DamageToArmorConversionRate + other2.DamageToArmorConversionRate) * FusedPropertyFactor;

                result = new ReactiveArmor(nextRarity, newDuration, newMovementSlowdown, newHealthIncrease, newDamageToArmorConversionRate);
                return true;
            }

            result = null;
            return false;
        }

        public override bool Use()
        {
            if (GameModeLoader.Access.GameMode == GameMode.Level &&
                SpecialEffectsMediator.Access.TryGetFirstEffectReceiver(out IReactiveArmorUser user) == true &&
                user.Use(this) == true)
            {
                HUDDisplay.Access.RegisterActiveItem(this);
                return true;
            }

            return false;
        }

        public override async UniTask<string> GetDescription()
        {
            LocalizedString title = new("Reactive armor", "Title");
            LocalizedString rarity = new("Rarity", Rarity.ToString());
            LocalizedString stats = new("Reactive armor", "Stats") { Arguments = new[] { this } };
            LocalizedString description = new("Reactive armor", "Description");

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

        public override bool Equals(object obj) => Equals(obj as ReactiveArmor);

        public bool Equals(ReactiveArmor other) => base.Equals(other) &&
                                                   MovementSlowdown.Equals(other.MovementSlowdown) &&
                                                   HealthIncrease.Equals(other.HealthIncrease) &&
                                                   DamageToArmorConversionRate.Equals(other.DamageToArmorConversionRate);

        public override int GetHashCode() => base.GetHashCode() ^
                                             MovementSlowdown.GetHashCode() ^
                                             HealthIncrease.GetHashCode() ^
                                             DamageToArmorConversionRate.GetHashCode();
    }
}