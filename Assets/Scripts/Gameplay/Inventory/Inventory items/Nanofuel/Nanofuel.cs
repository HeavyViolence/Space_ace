using Newtonsoft.Json;
using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    public sealed class Nanofuel : InventoryItem, IEquatable<Nanofuel>
    {
        public const float MinSpeedIncrease = 1f;
        public const float MaxSpeedIncrease = 30f;

        [JsonIgnore]
        public override string Title => throw new NotImplementedException();

        [JsonIgnore]
        public override string Description => throw new NotImplementedException();

        [JsonIgnore]
        public override string Stats => throw new NotImplementedException();

        [JsonIgnore]
        public override float Worth => (base.Worth + SpeedIncrease * SpeedUnitWorth) * (float)(Rarity + 1);

        public float SpeedIncrease { get; }

        public Nanofuel(ItemRarity rerity, float duration, float flankingSpeedup) : base(rerity, duration)
        {
            SpeedIncrease = Mathf.Clamp(flankingSpeedup, MinSpeedIncrease, MaxSpeedIncrease);
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
                float newFlankingSpeedup = (SpeedIncrease + other1.SpeedIncrease + other2.SpeedIncrease) * FusedPropertyFactor;

                result = new Nanofuel(nextRarity, newDuration, newFlankingSpeedup);
                return true;
            }

            result = null;
            return false;
        }

        public override bool Use()
        {
            if (GameModeLoader.Access.GameState == Main.GameState.Level &&
                SpecialEffectsMediator.Access.TryGetFirstEffectReceiver(out INanofuelUser user) == true &&
                user.Use(this) == true)
            {
                HUDDisplay.Access.RegisterActiveItem(this);
                return true;
            }

            return false;
        }

        public override bool Equals(object obj) => Equals(obj as Nanofuel);

        public bool Equals(Nanofuel other) => base.Equals(other) && SpeedIncrease.Equals(other.SpeedIncrease);

        public override int GetHashCode() => base.GetHashCode() ^ SpeedIncrease.GetHashCode();
    }
}