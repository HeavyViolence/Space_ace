using SpaceAce.Architecture;
using SpaceAce.Main;
using SpaceAce.Visualization;
using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Exceptional,
        Exotic,
        Epic,
        Legendary
    }

    [Serializable]
    public abstract class InventoryItem
    {
        private const float LegendaryItemHighestSpawnProbability = 0.01f;
        protected const float FusedItemPropertyFactor = 0.618f;

        protected static readonly GameServiceFastAccess<EntityVisualizer> s_entityVisualizer = new();
        protected static readonly GameServiceFastAccess<GameModeLoader> s_gameModeLoader = new();

        [SerializeField] private ItemRarity _rarity;
        [SerializeField] private float _duration;
        [SerializeField] private int _scrapValue;

        public ItemRarity Rarity => _rarity;
        public Color32 RarityColor => s_entityVisualizer.Access.GetInventoryItemRarityColor(Rarity);
        public Sprite Icon => s_entityVisualizer.Access.GetInventoryItemIcon(GetType());
        public string Title => throw new NotImplementedException();
        public string Description => throw new NotImplementedException();
        public abstract string Stats { get; }
        public float Duration => _duration;
        public int ScrapValue => _scrapValue;
        public abstract bool UsableOutsideOfLevel { get; }

        public static float GetHighestSpawnProbabilityFromRarity(ItemRarity rarity)
        {
            int rarityValuesAmount = Enum.GetValues(typeof(ItemRarity)).Length;
            float numericLegendaryRarity = 1f - (float)ItemRarity.Legendary / rarityValuesAmount;
            float numericRarity = 1f - (float)rarity / rarityValuesAmount;
            float remappingPower = Mathf.Log(LegendaryItemHighestSpawnProbability, numericLegendaryRarity);

            return Mathf.Pow(numericRarity, remappingPower);
        }

        public static ItemRarity GetNextRarity(ItemRarity rarity) => (ItemRarity)Mathf.Clamp((int)rarity + 1, 0, (int)ItemRarity.Legendary);

        public static ItemRarity GetPreviousRarity(ItemRarity rarity) => (ItemRarity)Mathf.Clamp((int)rarity - 1, 0, (int)ItemRarity.Legendary);

        public InventoryItem(ItemRarity rarity,
                             float duration,
                             int scrapValue)
        {
            _rarity = rarity;
            _duration = duration;
            _scrapValue = scrapValue;
        }

        public abstract bool Use();
        public abstract bool Fuse(InventoryItem item1, InventoryItem item2, out InventoryItem result);
        public abstract int Scrap();
    }
}