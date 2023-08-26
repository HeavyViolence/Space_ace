using Newtonsoft.Json;
using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    public sealed class RepairKit : InventoryItem, IEquatable<RepairKit>
    {
        public const float MinRegenPerSecond = 10f;
        public const float MaxRegenPerSecond = 100f;

        [JsonIgnore]
        public override string Title => throw new NotImplementedException();

        [JsonIgnore]
        public override string Description => throw new NotImplementedException();

        [JsonIgnore]
        public override string Stats => throw new NotImplementedException();

        [JsonIgnore]
        public override float Worth => (base.Worth + RegenPerSecond * HealthUnitWorth * DurationUnitWorth) * (float)(Rarity + 1);

        public float RegenPerSecond { get; }

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
            else
            {
                result = null;
                return false;
            }
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

        public override bool Equals(object obj) => Equals(obj as RepairKit);

        public bool Equals(RepairKit other) => base.Equals(other) && RegenPerSecond.Equals(other.RegenPerSecond);

        public override int GetHashCode() => base.GetHashCode() ^ RegenPerSecond.GetHashCode();
    }
}