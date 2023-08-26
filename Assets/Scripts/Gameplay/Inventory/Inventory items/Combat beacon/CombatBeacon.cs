using Newtonsoft.Json;
using SpaceAce.Main;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    public sealed class CombatBeacon : InventoryItem, IEquatable<CombatBeacon>
    {
        public const int MinAdditionalEnemies = 1;
        public const int MaxAdditionalEnemies = 100;

        public const int MinAdditionalWaveLength = 0;
        public const int MaxAdditionalWaveLength = 10;

        [JsonIgnore]
        public override string Title => throw new NotImplementedException();

        [JsonIgnore]
        public override string Description => throw new NotImplementedException();

        [JsonIgnore]
        public override string Stats => throw new NotImplementedException();

        [JsonIgnore]
        public override float Worth => (AdditionalEnemies * AdditionalEnemyWorth +
                                        AdditionalWaveLength * AdditionalEnemyWorth) *
                                       (float)(Rarity + 1);

        [JsonIgnore]
        public override float Duration => float.PositiveInfinity;

        public int AdditionalEnemies { get; }

        public int AdditionalWaveLength { get; }

        public CombatBeacon(ItemRarity rarity,
                            int additionalEnemies,
                            int additionalWaveLength) : base(rarity, float.PositiveInfinity)
        {
            AdditionalEnemies = Mathf.Clamp(additionalEnemies, MinAdditionalEnemies, MaxAdditionalEnemies);
            AdditionalWaveLength = Mathf.Clamp(additionalWaveLength, MinAdditionalWaveLength, MaxAdditionalWaveLength);
        }

        public override bool Fuse(InventoryItem item1, InventoryItem item2, out InventoryItem result)
        {
            if (item1 is not null &&
                item2 is not null &&
                Rarity.Equals(item1.Rarity) &&
                Rarity.Equals(item2.Rarity) &&
                item1 is CombatBeacon other1 &&
                item2 is CombatBeacon other2)
            {
                ItemRarity nextRarity = GetNextRarity(Rarity);
                int newAdditionalEnemies = (int)((AdditionalEnemies + other1.AdditionalEnemies + other2.AdditionalEnemies) * FusedPropertyFactor);
                int newAdditionalWaveLength = (int)((AdditionalWaveLength + other1.AdditionalWaveLength + other2.AdditionalWaveLength) * FusedPropertyFactor);

                result = new CombatBeacon(nextRarity, newAdditionalEnemies, newAdditionalWaveLength);
                return true;
            }

            result = null;
            return false;
        }

        public override bool Use()
        {
            if (GameModeLoader.Access.GameMode == GameMode.Level &&
                SpecialEffectsMediator.Access.TryGetEffectReceivers(out IEnumerable<ICombatBeaconUser> users) == true)
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

        public override bool Equals(object obj) => Equals(obj as CombatBeacon);

        public bool Equals(CombatBeacon other) => base.Equals(other) &&
                                                  AdditionalEnemies.Equals(other.AdditionalEnemies) &&
                                                  AdditionalWaveLength.Equals(other.AdditionalWaveLength);

        public override int GetHashCode() => base.GetHashCode() ^
                                             AdditionalEnemies.GetHashCode() ^
                                             AdditionalWaveLength.GetHashCode();
    }
}