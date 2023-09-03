using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using UnityEngine;
using UnityEngine.Localization;

namespace SpaceAce.Gameplay.Inventories
{
    public sealed class Nanofuel : InventoryItem, IEquatable<Nanofuel>
    {
        public const float MinSpeedIncrease = 1f;
        public const float MaxSpeedIncrease = 30f;

        [JsonIgnore]
        public override float Worth => (base.Worth + SpeedIncrease * SpeedUnitWorth) * (float)(Rarity + 1);

        public float SpeedIncrease { get; }

        public Nanofuel(ItemRarity rerity, float duration, float speedIncrease) : base(rerity, duration)
        {
            SpeedIncrease = Mathf.Clamp(speedIncrease, MinSpeedIncrease, MaxSpeedIncrease);
        }

        public override bool Fuse(InventoryItem item1, InventoryItem item2, out InventoryItem result)
        {
            if (item1 is not null &&
                item2 is not null &&
                Rarity.Equals(item1.Rarity) &&
                Rarity.Equals(item2.Rarity) &&
                item1 is Nanofuel other1 &&
                item2 is Nanofuel other2)
            {
                ItemRarity nextRarity = GetNextRarity(Rarity);
                float newDuration = (Duration + other1.Duration + other2.Duration) * FusedPropertyFactor;
                float newSpeedIncrease = (SpeedIncrease + other1.SpeedIncrease + other2.SpeedIncrease) * FusedPropertyFactor;

                result = new Nanofuel(nextRarity, newDuration, newSpeedIncrease);
                return true;
            }

            result = null;
            return false;
        }

        public override bool Use()
        {
            if (GameModeLoader.Access.GameMode == Main.GameMode.Level &&
                SpecialEffectsMediator.Access.TryGetFirstEffectReceiver(out INanofuelUser user) == true &&
                user.Use(this) == true)
            {
                HUDDisplay.Access.RegisterActiveItem(this);
                return true;
            }

            return false;
        }

        public override async UniTask<string> GetDescription()
        {
            LocalizedString title = new("Nanofuel", "Title");
            LocalizedString rarity = new("Rarity", Rarity.ToString());
            LocalizedString stats = new("Nanofuel", "Stats") { Arguments = new[] { this } };
            LocalizedString description = new("Nanofuel", "Description");

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

        public override bool Equals(object obj) => Equals(obj as Nanofuel);

        public bool Equals(Nanofuel other) => base.Equals(other) && SpeedIncrease.Equals(other.SpeedIncrease);

        public override int GetHashCode() => base.GetHashCode() ^ SpeedIncrease.GetHashCode();
    }
}