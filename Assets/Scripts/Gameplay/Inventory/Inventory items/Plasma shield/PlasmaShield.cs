using Newtonsoft.Json;
using SpaceAce.Main;
using System;
using System.Collections.Generic;

namespace SpaceAce.Gameplay.Inventories
{
    public sealed class PlasmaShield : InventoryItem, IEquatable<PlasmaShield>
    {
        public const float MinArmorBoost = 100f;
        public const float MaxArmorBoost = 10000f;

        public const float MinProjectilesSlowdown = 0.1f;
        public const float MaxProjectilesSlowdown = 0.5f;

        public float ArmorBoost { get; private set; }
        public float ProjectilesSlowdown { get; private set; }

        [JsonIgnore]
        public override string Stats => throw new NotImplementedException();

        public PlasmaShield(ItemRarity rarity,
                            float duration,
                            int sellValue,
                            float armorBoost,
                            float projectilesSlowdown) : base(rarity, duration, sellValue)
        {
            if (armorBoost < MinArmorBoost ||
                armorBoost > MaxArmorBoost) throw new ArgumentOutOfRangeException(nameof(armorBoost));

            ArmorBoost = armorBoost;

            if (projectilesSlowdown < MinProjectilesSlowdown ||
                projectilesSlowdown > MaxProjectilesSlowdown) throw new ArgumentOutOfRangeException(nameof(projectilesSlowdown));

            ProjectilesSlowdown = projectilesSlowdown;
        }

        public override bool Use()
        {
            if (GameModeLoader.Access.GameState == GameState.Level &&
                SpecialEffectsMediator.TryGetEffectReceivers(out IEnumerable<IPlasmaShieldUser> users) == true)
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

        public override bool Fuse(InventoryItem item1, InventoryItem item2, out InventoryItem result)
        {
            if (item1 is not null &&
                item2 is not null &&
                item1 is PlasmaShield other1 &&
                item2 is PlasmaShield other2 &&
                item1.Rarity.Equals(Rarity) &&
                item2.Rarity.Equals(Rarity))
            {
                float newDuration = (Duration + other1.Duration) * FusedItemPropertyFactor;
                int newSellValue = (int)((SellValue + other1.SellValue + other2.SellValue) * FusedItemPropertyFactor);
                float newArmorBoost = (ArmorBoost + other1.ArmorBoost + other2.ArmorBoost) * FusedItemPropertyFactor;
                float newProjectilesSlowdown = (ProjectilesSlowdown + other1.ProjectilesSlowdown + other2.ProjectilesSlowdown) * FusedItemPropertyFactor;

                result = new PlasmaShield(GetNextRarity(Rarity), newDuration, newSellValue, newArmorBoost, newProjectilesSlowdown);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public override bool Equals(object obj) => Equals(obj as PlasmaShield);

        public bool Equals(PlasmaShield other) => base.Equals(other) &&
                                                  ArmorBoost.Equals(other.ArmorBoost) &&
                                                  ProjectilesSlowdown.Equals(other.ProjectilesSlowdown);

        public override int GetHashCode() => base.GetHashCode() ^ ArmorBoost.GetHashCode() ^ ProjectilesSlowdown.GetHashCode();
    }
}