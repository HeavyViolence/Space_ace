using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    public abstract class InventoryItemConfig : ScriptableObject
    {
        [SerializeField] private ItemRarity _rarity;

        [SerializeField] private float _duration = InventoryItem.MinDuration;
        [SerializeField] private float _durationRandomDeviation = 0f;

        public ItemRarity Rarity => _rarity;
        public float SpawnProbability => InventoryItem.GetHighestSpawnProbabilityFromRarity(Rarity);
        protected RangedFloat Duration { get; private set; }

        private void OnEnable()
        {
            ApplySettings();
        }

        public virtual void ApplySettings()
        {
            Duration = new(_duration, _durationRandomDeviation, InventoryItem.MinDuration, InventoryItem.MaxDuration);
        }

        public abstract InventoryItem GetItem();
    }
}