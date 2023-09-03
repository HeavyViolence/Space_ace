using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using UnityEngine;
using UnityEngine.Localization;

namespace SpaceAce.Gameplay.Inventories
{
    public sealed class RepairKit : InventoryItem, IEquatable<RepairKit>
    {
        public const float MinRegenPerSecond = 10f;
        public const float MaxRegenPerSecond = 100f;

        [JsonIgnore]
        public override float Worth => (base.Worth + RegenTotal * HealthUnitWorth) * (float)(Rarity + 1);

        public float RegenPerSecond { get; }

        [JsonIgnore]
        public float RegenTotal => RegenPerSecond * Duration;

        public RepairKit(ItemRarity rarity, float duration, float regenPerSecond) : base(rarity, duration)
        {
            RegenPerSecond = Mathf.Clamp(regenPerSecond, MinRegenPerSecond, MaxRegenPerSecond);
        }

        public override bool Fuse(InventoryItem item1, InventoryItem item2, out InventoryItem result)
        {
            if (item1 is not null &&
                item2 is not null &&
                Rarity.Equals(item1.Rarity) &&
                Rarity.Equals(item2.Rarity) &&
                item1 is RepairKit other1 &&
                item2 is RepairKit other2)
            {
                ItemRarity nextRarity = GetNextRarity(Rarity);
                float newDuration = (Duration + other1.Duration) * FusedPropertyFactor;
                float newRegenPerSecond = (RegenPerSecond + other1.RegenPerSecond + other2.RegenPerSecond) * FusedPropertyFactor;

                result = new RepairKit(nextRarity, newDuration, newRegenPerSecond);
                return true;
            }

            result = null;
            return false;
        }

        public override bool Use()
        {
            if (GameModeLoader.Access.GameMode == Main.GameMode.Level &&
                SpecialEffectsMediator.Access.TryGetFirstEffectReceiver(out IRepairKitUser user) == true &&
                user.Use(this) == true)
            {
                HUDDisplay.Access.RegisterActiveItem(this);
                return true;
            }

            return false;
        }

        public override async UniTask<string> GetDescription()
        {
            LocalizedString title = new("Repair kit", "Title");
            LocalizedString rarity = new("Rarity", Rarity.ToString());
            LocalizedString stats = new("Repair kit", "Stats") { Arguments = new[] { this } };
            LocalizedString description = new("Repair kit", "Description");

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

        public override bool Equals(object obj) => Equals(obj as RepairKit);

        public bool Equals(RepairKit other) => base.Equals(other) && RegenPerSecond.Equals(other.RegenPerSecond);

        public override int GetHashCode() => base.GetHashCode() ^ RegenPerSecond.GetHashCode();
    }
}