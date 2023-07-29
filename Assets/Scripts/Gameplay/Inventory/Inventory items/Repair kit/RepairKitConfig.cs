using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    [CreateAssetMenu(fileName = "Repair kit", menuName = "Space ace/Configs/Loot/Repair kit")]
    public sealed class RepairKitConfig : InventoryItemConfig
    {
        [SerializeField] private float _regenPerSecond = RepairKit.MinRegenPerSecond;
        [SerializeField] private float _regenPerSecondRandomDeviation = 0f;

        public RangedFloat RegenPerSecond { get; private set; }

        public override void ApplySettings()
        {
            base.ApplySettings();

            RegenPerSecond = new(_regenPerSecond, _regenPerSecondRandomDeviation, RepairKit.MinRegenPerSecond, RepairKit.MaxRegenPerSecond);
        }

        public override InventoryItem GetItem() => new RepairKit(Rarity, Duration.RandomValue, RegenPerSecond.RandomValue);
    }
}