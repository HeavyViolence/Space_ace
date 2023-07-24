using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    public abstract class InventoryItemConfig : ScriptableObject
    {
        [SerializeField] private ItemRarity _rarity;

        [SerializeField] private int _sellValue = InventoryItem.MinSellValue;
        [SerializeField] private int _sellValueRandomDeviation = 0;

        [SerializeField] private float _duration = InventoryItem.MinDuration;
        [SerializeField] private float _durationRandomDeviation = 0f;

        public ItemRarity Rarity => _rarity;
        public float SpawnProbability => InventoryItem.GetHighestSpawnProbabilityFromRarity(Rarity);
        protected RangedInt SellValue { get; private set; }
        protected RangedFloat Duration { get; private set; }

        private void OnEnable()
        {
            ApplySettings();
        }

        public virtual void ApplySettings()
        {
            SellValue = new(_sellValue, _sellValueRandomDeviation, InventoryItem.MinSellValue, InventoryItem.MaxSellValue);
            Duration = new(_duration, _durationRandomDeviation, InventoryItem.MinDuration, InventoryItem.MaxDuration);
        }

        public abstract InventoryItem GetItem();
    }
}