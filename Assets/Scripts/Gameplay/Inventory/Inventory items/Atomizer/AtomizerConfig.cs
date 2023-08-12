using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    [CreateAssetMenu(fileName = "Atomizer", menuName = "Space ace/Configs/Loot/Atomizer")]
    public sealed class AtomizerConfig : InventoryItemConfig
    {
        [SerializeField] private int _entitiesToBeDestroyed = Atomizer.MinEntitiesToBeDestroyed;
        [SerializeField] private int _entitiesToBeDestroyedRandomDeviation = 0;

        public RangedInt EntitiesToBeDestroyedAmount { get; private set; }

        public override void ApplySettings()
        {
            base.ApplySettings();

            EntitiesToBeDestroyedAmount = new(_entitiesToBeDestroyed,
                                              _entitiesToBeDestroyedRandomDeviation,
                                              Atomizer.MinEntitiesToBeDestroyed,
                                              Atomizer.MaxEntitiesToBeDestroyed);
        }

        public override InventoryItem GetItem() => new Atomizer(Rarity,
                                                                Duration.RandomValue,
                                                                EntitiesToBeDestroyedAmount.RandomValue);
    }
}