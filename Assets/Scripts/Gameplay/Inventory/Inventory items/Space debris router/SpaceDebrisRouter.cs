using Newtonsoft.Json;
using SpaceAce.Main;
using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    public sealed class SpaceDebrisRouter : InventoryItem, IEquatable<SpaceDebrisRouter>
    {
        public const float MinSpaceDebrisSpawnSpeedup = 1f;
        public const float MaxSpaceDebrisSpawnSpeedup = 10f;

        [JsonIgnore]
        public override string Title => throw new NotImplementedException();

        [JsonIgnore]
        public override string Description => throw new NotImplementedException();

        [JsonIgnore]
        public override string Stats => throw new NotImplementedException();

        [JsonIgnore]
        public override float Worth => (base.Worth + SpaceDebrisSpawnSpeedup * SpawnSpeedupUnitWorth) * (float)(Rarity + 1);

        public float SpaceDebrisSpawnSpeedup { get; }

        public SpaceDebrisRouter(ItemRarity rarity,
                                 float duration,
                                 float spaceDebrisSpawnSpeedup) : base(rarity, duration)
        {
            SpaceDebrisSpawnSpeedup = Mathf.Clamp(spaceDebrisSpawnSpeedup, MinSpaceDebrisSpawnSpeedup, MaxSpaceDebrisSpawnSpeedup);
        }

        public override bool Fuse(InventoryItem item1, InventoryItem item2, out InventoryItem result)
        {
            if (item1 is not null &&
                item2 is not null &&
                Rarity.Equals(item1.Rarity) &&
                Rarity.Equals(item2.Rarity) &&
                item1 is SpaceDebrisRouter other1 &&
                item2 is SpaceDebrisRouter other2)
            {
                ItemRarity nextRarity = GetNextRarity(Rarity);
                float newDuration = (Duration + other1.Duration + other2.Duration) * FusedPropertyFactor;
                float newSpaceDebrisSpawnSpeedup = (SpaceDebrisSpawnSpeedup +
                                                    other1.SpaceDebrisSpawnSpeedup +
                                                    other2.SpaceDebrisSpawnSpeedup) *
                                                   FusedPropertyFactor;

                result = new SpaceDebrisRouter(nextRarity, newDuration, newSpaceDebrisSpawnSpeedup);
                return true;
            }

            result = null;
            return false;
        }

        public override bool Use()
        {
            if (GameModeLoader.Access.GameMode == GameMode.Level &&
                SpecialEffectsMediator.Access.TryGetFirstEffectReceiver(out ISpaceDebrisRouterUser user) == true &&
                user.Use(this) == true)
            {
                HUDDisplay.Access.RegisterActiveItem(this);
                return true;
            }

            return false;
        }

        public override bool Equals(object obj) => Equals(obj as SpaceDebrisRouter);

        public bool Equals(SpaceDebrisRouter other) => base.Equals(other) && SpaceDebrisSpawnSpeedup.Equals(other.SpaceDebrisSpawnSpeedup);

        public override int GetHashCode() => base.GetHashCode() ^ SpaceDebrisSpawnSpeedup.GetHashCode();
    }
}