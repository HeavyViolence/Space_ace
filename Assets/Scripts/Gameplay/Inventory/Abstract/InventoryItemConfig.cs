using SpaceAce.Auxiliary;
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

    public abstract class InventoryItemConfig : ScriptableObject
    {
        private const float LegendaryItemHighestSpawnProbability = 0.01f;

        public const float MinDuration = 1f;
        public const float MaxDuration = 30f;
        public const float DefaultDuration = 5f;

        [SerializeField] private ItemRarity _rarity;

        [SerializeField] private float _duration = DefaultDuration;
        [SerializeField] private float _durationRandomDeviation = 0f;

        protected RangedFloat Duration { get; private set; }

        private void OnEnable()
        {
            ApplySettings();
        }

        public virtual void ApplySettings()
        {
            Duration = new(_duration, _durationRandomDeviation);
        }

        public float GetSpawnProbability()
        {
            int rarityValuesAmount = Enum.GetValues(typeof(ItemRarity)).Length;
            float legendaryRarity = 1f - (float)ItemRarity.Legendary / rarityValuesAmount;
            float rarity = 1f - (float)_rarity / rarityValuesAmount;
            float remappingPower = Mathf.Log(LegendaryItemHighestSpawnProbability, legendaryRarity);

            return Mathf.Pow(rarity, remappingPower);
        }

        public abstract IInventoryItem GetItem();
    }
}