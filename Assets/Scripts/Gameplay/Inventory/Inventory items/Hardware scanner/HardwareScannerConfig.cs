using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    [CreateAssetMenu(fileName = "Hardware scanner", menuName = "Space ace/Configs/Loot/Hardware scanner")]
    public sealed class HardwareScannerConfig : InventoryItemConfig
    {
        [SerializeField] private float _hardwareSpawnProbabilityIncrease = HardwareScanner.MinHardawareSpawnProbabilityIncrease;
        [SerializeField] private float _hardwareSpawnProbabilityIncreaseRandomDeviation = 0f;

        public RangedFloat HardwareSpawnProbabilityIncrease { get; private set; }

        public override void ApplySettings()
        {
            base.ApplySettings();

            HardwareSpawnProbabilityIncrease = new(_hardwareSpawnProbabilityIncrease,
                                                   _hardwareSpawnProbabilityIncreaseRandomDeviation,
                                                   HardwareScanner.MinHardawareSpawnProbabilityIncrease,
                                                   HardwareScanner.MaxHardwareSpawnProbabilityIncrease);
        }

        public override InventoryItem GetItem() => new HardwareScanner(Rarity,
                                                                       Duration.RandomValue,
                                                                       HardwareSpawnProbabilityIncrease.RandomValue);
    }
}