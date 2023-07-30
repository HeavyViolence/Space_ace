using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    [CreateAssetMenu(fileName = "Nanofuel", menuName = "Space ace/Configs/Loot/Nanofuel")]
    public sealed class NanofuelConfig : InventoryItemConfig
    {
        [SerializeField] private float _speedIncrease = Nanofuel.MinSpeedIncrease;
        [SerializeField] private float _speedIncreaseRandomDeviation = 0f;

        public RangedFloat SpeedIncrease { get; private set; }

        public override void ApplySettings()
        {
            base.ApplySettings();

            SpeedIncrease = new(_speedIncrease, _speedIncreaseRandomDeviation, Nanofuel.MinSpeedIncrease, Nanofuel.MaxSpeedIncrease);
        }

        public override InventoryItem GetItem() => new Nanofuel(Rarity, Duration.RandomValue, SpeedIncrease.RandomValue);
    }
}