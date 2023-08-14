using Newtonsoft.Json;
using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    public sealed class Ore : InventoryItem, IEquatable<Ore>
    {
        public const float MinDensity = 0f;
        public const float MaxDensity = 100f;

        [JsonIgnore]
        public override string Title => throw new NotImplementedException();

        [JsonIgnore]
        public override string Description => throw new NotImplementedException();

        [JsonIgnore]
        public override string Stats => throw new NotImplementedException();

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

        public override bool Equals(object obj) => Equals(obj as Ore);

        public bool Equals(Ore other) => base.Equals(other) && Density.Equals(other.Density);

        public override int GetHashCode() => base.GetHashCode() ^ Density.GetHashCode();
    }
}