using Newtonsoft.Json;
using SpaceAce.Architecture;
using SpaceAce.Gameplay.Players;
using SpaceAce.Main;
using SpaceAce.UI;
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

    public abstract class InventoryItem: IEquatable<InventoryItem>
    {
        public const int MinSellValue = 10;
        public const int MaxSellValue = 10000;

        public const float MinDuration = 15f;
        public const float MaxDuration = 300f;

        public const int ItemsPerInfusion = 3;

        private const float LegendaryItemHighestSpawnProbability = 0.01f;
        protected const float FusedItemPropertyFactor = 0.618f;

        protected static readonly GameServiceFastAccess<EntityVisualizer> EntityVisualizer = new();
        protected static readonly GameServiceFastAccess<GameModeLoader> GameModeLoader = new();
        protected static readonly GameServiceFastAccess<Player> Player = new();
        protected static readonly GameServiceFastAccess<HUDDisplay> HUDDisplay = new();

        public ItemRarity Rarity { get; private set; }

        [JsonIgnore]
        public Color32 RarityColor => EntityVisualizer.Access.GetInventoryItemRarityColor(Rarity);

        [JsonIgnore]
        public Sprite Icon => EntityVisualizer.Access.GetInventoryItemIcon(GetType());

        [JsonIgnore]
        public string Title => throw new NotImplementedException();

        [JsonIgnore]
        public string Description => throw new NotImplementedException();

        [JsonIgnore]
        public abstract string Stats { get; }

        public float Duration { get; private set; }
        public int SellValue { get; private set; }

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

        public InventoryItem(ItemRarity rarity, float duration, int sellValue)
        {
            Rarity = rarity;

            if (duration < MinDuration || duration > MaxDuration) throw new ArgumentOutOfRangeException(nameof(duration));
            Duration = duration;

            if (sellValue < MinSellValue || sellValue > MaxSellValue) throw new ArgumentOutOfRangeException(nameof(sellValue));
            SellValue = sellValue;
        }

        public void Sell() => Player.Access.Wallet.AddCredits(SellValue);

        public abstract bool Use();
        public abstract bool Fuse(InventoryItem item1, InventoryItem item2, out InventoryItem result);

        public override bool Equals(object obj) => Equals(obj as InventoryItem);

        public bool Equals(InventoryItem other) => other is not null &&
                                                   Rarity.Equals(other.Rarity) &&
                                                   Duration.Equals(other.Duration) &&
                                                   SellValue.Equals(other.SellValue);

        public override int GetHashCode() => Rarity.GetHashCode() ^ Duration.GetHashCode() ^ SellValue.GetHashCode();
    }
}