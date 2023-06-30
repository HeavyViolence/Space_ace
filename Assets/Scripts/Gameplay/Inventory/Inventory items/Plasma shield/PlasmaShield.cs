using SpaceAce.Main;
using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    [Serializable]
    public sealed class PlasmaShield : InventoryItem, IEquatable<PlasmaShield>
    {
        [SerializeField] private float _armorBoost;

        public float ArmorBoost => _armorBoost;
        public override string Stats => throw new NotImplementedException();
        public override bool UsableOutsideOfLevel => false;

        public PlasmaShield(ItemRarity rarity,
                            float duration,
                            int worth,
                            float armorBoost) : base(rarity, duration, worth)
        {
            _armorBoost = armorBoost;
        }

        public override bool Use()
        {
            if (s_gameModeLoader.Access.GameState == GameState.Level &&
                SpecialEffectsMediator.TryGetFirstEffectReceiver(out IPlasmaShieldUser user) == true)
            {
                return user.Use(this);
            }

            return false;
        }

        public override bool Fuse(InventoryItem item1, InventoryItem item2, out InventoryItem result)
        {
            if (item1 is not null && item2 is not null &&
                item1 is PlasmaShield other1 && item2 is PlasmaShield other2 &&
                item1.Rarity.Equals(Rarity) && item2.Rarity.Equals(Rarity))
            {
                float newDuration = (Duration + other1.Duration) * FusedItemPropertyFactor;
                int newScrapValue = (int)((Worth + other1.Worth + other2.Worth) * FusedItemPropertyFactor);
                float newArmorBoost = (ArmorBoost + other1.ArmorBoost + other2.ArmorBoost) * FusedItemPropertyFactor;

                result = new PlasmaShield(GetNextRarity(Rarity), newDuration, newScrapValue, newArmorBoost);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public override bool Equals(object obj) => Equals(obj as PlasmaShield);

        public bool Equals(PlasmaShield other) => other is not null &&
                                                  other.Rarity.Equals(Rarity) &&
                                                  other.ArmorBoost.Equals(ArmorBoost) &&
                                                  other.Duration.Equals(Duration) &&
                                                  other.Worth.Equals(Worth);

        public override int GetHashCode() => Rarity.GetHashCode() ^
                                             ArmorBoost.GetHashCode() ^
                                             Duration.GetHashCode() ^
                                             Worth.GetHashCode();
    }
}