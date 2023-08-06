using Newtonsoft.Json;
using SpaceAce.Main;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    public sealed class AntimatterAmmo : InventoryItem, IEquatable<AntimatterAmmo>
    {
        public const float MinConsecutiveDamageFactor = 1f;
        public const float MaxConsecutiveDamageFactor = 2f;

        [JsonIgnore]
        public override string Title => throw new NotImplementedException();

        [JsonIgnore]
        public override string Description => throw new NotImplementedException();

        [JsonIgnore]
        public override string Stats => throw new NotImplementedException();

        [JsonIgnore]
        public override float Worth => (base.Worth + ConsecutiveDamageFactor * ConsecutiveDamageFactorUnitWorth) * (float)(Rarity + 1);

        public float ConsecutiveDamageFactor { get; private set; }

        public AntimatterAmmo(ItemRarity rarity, float duration, float consecutiveDamageFactor) : base(rarity, duration)
        {
            ConsecutiveDamageFactor = Mathf.Clamp(consecutiveDamageFactor, MinConsecutiveDamageFactor, MaxConsecutiveDamageFactor);
        }

        public override bool Fuse(InventoryItem item1, InventoryItem item2, out InventoryItem result)
        {
            if (item1 is not null &&
                item2 is not null &&
                Rarity.Equals(item1.Rarity) &&
                Rarity.Equals(item2.Rarity) &&
                item1 is AntimatterAmmo other1 &&
                item2 is AntimatterAmmo other2)
            {
                ItemRarity nextRarity = GetNextRarity(Rarity);
                float newDuration = (Duration + other1.Duration + other2.Duration) * FusedPropertyFactor;
                float newConsecutiveDamageFactor = (ConsecutiveDamageFactor + other1.ConsecutiveDamageFactor + other2.ConsecutiveDamageFactor) * FusedPropertyFactor;

                result = new AntimatterAmmo(nextRarity, newDuration, newConsecutiveDamageFactor);
                return true;
            }

            result = null;
            return false;
        }

        public override bool Use()
        {
            if (GameModeLoader.Access.GameState == GameState.Level &&
                SpecialEffectsMediator.Access.TryGetEffectReceivers(out IEnumerable<IAntimatterAmmoUser> users) == true)
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

        public override bool Equals(object obj) => Equals(obj as AntimatterAmmo);

        public bool Equals(AntimatterAmmo other) => base.Equals(other) && ConsecutiveDamageFactor.Equals(other.ConsecutiveDamageFactor);

        public override int GetHashCode() => base.GetHashCode() ^ ConsecutiveDamageFactor.GetHashCode();
    }
}