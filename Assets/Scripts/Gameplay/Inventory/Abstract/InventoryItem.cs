using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SpaceAce.Architecture;
using SpaceAce.Auxiliary;
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
        public const float DurationUnitWorth = 0.8f;
        public const float HealthUnitWorth = 0.3f;
        public const float ArmorUnitWorth = 0.5f;
        public const float PlayerSlowdownUnitWorth = -100f;
        public const float SlowdownUnitWorth = 100f;
        public const float ConversionUnitWorth = 100f;
        public const float SpeedUnitWorth = 10f;
        public const float AmmoHomingSpeedUnitWorth = 5f;
        public const float AmmoSpeedBoostUnitWorth = 1000f;
        public const float ConsecutiveDamageFactorUnitWorth = 300f;
        public const float CooldownReductionUnitWorth = 500f;
        public const float CooldownIncreaseUnitWorth = -500f;
        public const float FireRateBoostUnitWorth = 500f;
        public const float DamageBoostUnitWorth = 500f;
        public const float JamProbabilityUnitWorth = 800f;
        public const float AdditionalEnemyWorth = 50f;
        public const float KillWorth = 100f;
        public const float SpawnSpeedupUnitWorth = 80f;
        public const float OreDensityUnitWorth = 30f;
        public const float ItemSpawnProbabilityUnitWorth = 800f;
        public const float ExperienceToCreditsConversionRateUnitWorth = 1300f;
        public const float ExperienceBoostUnitWorth = 1300f;
        public const float ExperienceDepletionSlowdownUnitWorth = 800f;

        public const float MinDuration = 10f;
        public const float MaxDuration = 600f;

        public const int ItemsPerInfusion = 3;

        private const float LegendaryItemHighestSpawnProbability = 0.01f;
        protected const float FusedPropertyFactor = 0.618f;

        protected static readonly GameServiceFastAccess<EntityVisualizer> EntityVisualizer = new();
        protected static readonly GameServiceFastAccess<GameModeLoader> GameModeLoader = new();
        protected static readonly GameServiceFastAccess<Player> Player = new();
        protected static readonly GameServiceFastAccess<HUDDisplay> HUDDisplay = new();
        protected static readonly GameServiceFastAccess<GamePauser> GamePauser = new();
        protected static readonly GameServiceFastAccess<SpecialEffectsMediator> SpecialEffectsMediator = new();

        public ItemRarity Rarity { get; private set; }

        [JsonIgnore]
        public Color32 RarityColor => EntityVisualizer.Access.GetInventoryItemRarityColor(Rarity);

        [JsonIgnore]
        public Sprite Icon => EntityVisualizer.Access.GetInventoryItemIcon(GetType());

        [JsonIgnore]
        public virtual float Worth => Duration * DurationUnitWorth;

        public virtual float Duration { get; }

        [JsonIgnore]
        public string FormattedDuration => AuxMath.GetFormattedTime(Duration);

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

        public InventoryItem(ItemRarity rarity, float duration)
        {
            Rarity = rarity;
            Duration = Mathf.Clamp(duration, MinDuration, MaxDuration);
        }

        public abstract bool Use();

        public abstract bool Fuse(InventoryItem item1, InventoryItem item2, out InventoryItem result);

        public abstract UniTask<string> GetDescription();

        public override bool Equals(object obj) => Equals(obj as InventoryItem);

        public bool Equals(InventoryItem other) => other is not null && Rarity.Equals(other.Rarity) && Duration.Equals(other.Duration);

        public override int GetHashCode() => Rarity.GetHashCode() ^ Duration.GetHashCode();
    }
}