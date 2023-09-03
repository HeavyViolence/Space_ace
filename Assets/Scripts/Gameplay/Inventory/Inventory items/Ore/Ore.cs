using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using UnityEngine;
using UnityEngine.Localization;

namespace SpaceAce.Gameplay.Inventories
{
    public sealed class Ore : InventoryItem, IEquatable<Ore>
    {
        public const float MinDensity = 0f;
        public const float MaxDensity = 100f;

        [JsonIgnore]
        public override float Duration => 0f;

        [JsonIgnore]
        public override float Worth => Density * OreDensityUnitWorth * (float)(Rarity + 1);

        public float Density { get; }

        public Ore(ItemRarity rarity, float density) : base(rarity, 0f)
        {
            Density = Mathf.Clamp(density, MinDensity, MaxDensity);
        }

        public override bool Fuse(InventoryItem item1, InventoryItem item2, out InventoryItem result)
        {
            if (item1 is not null &&
                item2 is not null &&
                Rarity.Equals(item1.Rarity) &&
                Rarity.Equals(item2.Rarity) &&
                item1 is Ore other1 &&
                item2 is Ore other2)
            {
                ItemRarity nextRarity = GetNextRarity(Rarity);
                float newDensity = (Density + other1.Density + other2.Density) * FusedPropertyFactor;

                result = new Ore(nextRarity, newDensity);
                return true;
            }

            result = null;
            return false;
        }

        public override bool Use() => false;

        public override async UniTask<string> GetDescription()
        {
            LocalizedString title = new("Ore", "Title");
            LocalizedString rarity = new("Rarity", Rarity.ToString());
            LocalizedString stats = new("Ore", "Stats") { Arguments = new[] { this } };
            LocalizedString description = new("Ore", "Description");

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

        public override bool Equals(object obj) => Equals(obj as Ore);

        public bool Equals(Ore other) => base.Equals(other) && Density.Equals(other.Density);

        public override int GetHashCode() => base.GetHashCode() ^ Density.GetHashCode();
    }
}