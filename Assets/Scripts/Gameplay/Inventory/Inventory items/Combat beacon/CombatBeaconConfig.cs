using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    [CreateAssetMenu(fileName = "Combat beacon", menuName = "Space ace/Configs/Loot/Combat beacon")]
    public sealed class CombatBeaconConfig : InventoryItemConfig
    {
        [SerializeField] private int _additionalEnemies = CombatBeacon.MinAdditionalEnemies;
        [SerializeField] private int _additionalEnemiesRandomDeviation = 0;

        [SerializeField] private int _additionalWaveLength = CombatBeacon.MinAdditionalWaveLength;
        [SerializeField] private int _additionalWaveLengthRandomDeviation = 0;

        public RangedInt AdditionalEnemies { get; private set; }

        public RangedInt AdditionalWaveLength { get; private set; }

        public override void ApplySettings()
        {
            base.ApplySettings();

            AdditionalEnemies = new(_additionalEnemies,
                                    _additionalEnemiesRandomDeviation,
                                    CombatBeacon.MinAdditionalEnemies,
                                    CombatBeacon.MaxAdditionalEnemies);

            AdditionalWaveLength = new(_additionalWaveLength,
                                       _additionalWaveLengthRandomDeviation,
                                       CombatBeacon.MinAdditionalWaveLength,
                                       CombatBeacon.MaxAdditionalWaveLength);
        }

        public override InventoryItem GetItem() => new CombatBeacon(Rarity,
                                                                    AdditionalEnemies.RandomValue,
                                                                    AdditionalWaveLength.RandomValue);
    }
}