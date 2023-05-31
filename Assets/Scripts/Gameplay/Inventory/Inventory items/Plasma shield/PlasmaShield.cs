using SpaceAce.Main;
using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventory
{
    [Serializable]
    public sealed class PlasmaShield : InventoryItem, IEquatable<PlasmaShield>
    {
        [SerializeField] private float _armorBoost;

        public float ArmorBoost => _armorBoost;
        public override string Stats => throw new NotImplementedException();

        public PlasmaShield(ItemRarity rarity,
                            float duration,
                            int scrapValue,
                            float armorBoost) : base(rarity, duration, scrapValue)
        {
            _armorBoost = armorBoost;
        }

        public override bool Use()
        {
            if (GameModeLoader.Access.GameState == GameState.Level &&
                SpecialEffectsMediator.TryGetFirstEffectReceiver(out IPlasmaShieldUser user) == true)
            {
                return user.Use(this);
            }

            return false;
        }

        public override bool Fuse(InventoryItem pair, out InventoryItem result)
        {
            if (pair is not null &&
                pair is PlasmaShield other &&
                pair.Rarity.Equals(Rarity))
            {
                var newRarity = (ItemRarity)Mathf.Clamp((int)Rarity + 1, 0, (int)ItemRarity.Legendary);
                float newDuration = (Duration + other.Duration) * FusedItemPropertyFactor;
                int newScrapValue = (int)((ScrapValue + other.ScrapValue) * FusedItemPropertyFactor);
                float newArmorBoost = (ArmorBoost + other.ArmorBoost) * FusedItemPropertyFactor;

                result = new PlasmaShield(newRarity, newDuration, newScrapValue, newArmorBoost);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public override int Scrap() => ScrapValue;

        public override bool Equals(object obj) => Equals(obj as PlasmaShield);

        public bool Equals(PlasmaShield other) => other is not null &&
                                                  other.Rarity.Equals(Rarity) &&
                                                  other.ArmorBoost.Equals(ArmorBoost) &&
                                                  other.Duration.Equals(Duration) &&
                                                  other.ScrapValue.Equals(ScrapValue);

        public override int GetHashCode() => Rarity.GetHashCode() ^
                                             ArmorBoost.GetHashCode() ^
                                             Duration.GetHashCode() ^
                                             ScrapValue.GetHashCode();
    }
}