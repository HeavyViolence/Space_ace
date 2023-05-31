using SpaceAce.Architecture;
using SpaceAce.Main;
using SpaceAce.Visualization;
using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventory
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

        protected static readonly GameServiceFastAccess<EntityVisualizer> EntityVisualizer = new();
        protected static readonly GameServiceFastAccess<GameModeLoader> GameModeLoader = new();

        [SerializeField] private ItemRarity _rarity;
        [SerializeField] private float _duration;
        [SerializeField] private int _scrapValue;

        public ItemRarity Rarity => _rarity;
        public Color32 RarityColor => EntityVisualizer.Access.GetInventoryItemRarityColor(Rarity);
        public Sprite Icon => EntityVisualizer.Access.GetInventoryItemIcon(GetType());
        public string Title => throw new NotImplementedException();
        public string Description => throw new NotImplementedException();
        public abstract string Stats { get; }
        public float Duration => _duration;
        public int ScrapValue => _scrapValue;

        public static float GetHighestSpawnProbabilityFromRarity(ItemRarity rarity)
        {
            int rarityValuesAmount = Enum.GetValues(typeof(ItemRarity)).Length;
            float numericLegendaryRarity = 1f - (float)ItemRarity.Legendary / rarityValuesAmount;
            float numericRarity = 1f - (float)rarity / rarityValuesAmount;
            float remappingPower = Mathf.Log(LegendaryItemHighestSpawnProbability, numericLegendaryRarity);

            return Mathf.Pow(numericRarity, remappingPower);
        }

        public InventoryItem(ItemRarity rarity,
                             float duration,
                             int scrapValue)
        {
            _rarity = rarity;
            _duration = duration;
            _scrapValue = scrapValue;
        }

        public abstract bool Use();
        public abstract bool Fuse(InventoryItem pair, out InventoryItem result);
        public abstract int Scrap();
    }
}