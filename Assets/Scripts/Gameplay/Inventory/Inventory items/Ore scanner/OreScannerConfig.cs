using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    [CreateAssetMenu(fileName = "Ore scanner", menuName = "Space ace/Configs/Loot/Ore scanner")]
    public sealed class OreScannerConfig : InventoryItemConfig
    {
        [SerializeField] private float _oreSpawnProbabilityIncrease = OreScanner.MinOreSpawnProbabilityIncrease;
        [SerializeField] private float _oreSpawnProbabilityIncreaseRandomDeviation = 0f;

        public RangedFloat OreSpawnProbabilityIncrease { get; private set; }

        public override void ApplySettings()
        {
            base.ApplySettings();

            OreSpawnProbabilityIncrease = new(_oreSpawnProbabilityIncrease,
                                              _oreSpawnProbabilityIncreaseRandomDeviation,
                                              OreScanner.MinOreSpawnProbabilityIncrease,
                                              OreScanner.MaxOreSpawnProbabilityIncrease);
        }

        public override InventoryItem GetItem() => new OreScanner(Rarity,
                                                                  Duration.RandomValue,
                                                                  OreSpawnProbabilityIncrease.RandomValue);
    }
}