using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    public abstract class InventoryItemConfig : ScriptableObject
    {
        public const int MinScrapValue = 10;
        public const int MaxScrapValue = 10000;

        public const float MinDuration = 1f;
        public const float MaxDuration = 60f;
        public const float DefaultDuration = 5f;

        [SerializeField] private ItemRarity _rarity;

        [SerializeField] private int _scrapValue = MinScrapValue;
        [SerializeField] private int _scrapValueRandomDeviation = 0;

        [SerializeField] private float _duration = DefaultDuration;
        [SerializeField] private float _durationRandomDeviation = 0f;

        public ItemRarity Rarity => _rarity;
        public float SpawnProbability => InventoryItem.GetHighestSpawnProbabilityFromRarity(Rarity);
        protected RangedInt ScrapValue { get; private set; }
        protected RangedFloat Duration { get; private set; }

        private void OnEnable()
        {
            ApplySettings();
        }

        public virtual void ApplySettings()
        {
            ScrapValue = new(_scrapValue, _scrapValueRandomDeviation);
            Duration = new(_duration, _durationRandomDeviation);
        }

        public abstract InventoryItem GetItem();
    }
}