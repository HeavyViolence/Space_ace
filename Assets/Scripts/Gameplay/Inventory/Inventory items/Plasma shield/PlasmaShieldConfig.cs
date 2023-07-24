using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    [CreateAssetMenu(fileName = "Plasma shield", menuName = "Space ace/Configs/Loot/Plasma shield")]
    public sealed class PlasmaShieldConfig : InventoryItemConfig
    {
        [SerializeField] private float _armorBoost = PlasmaShield.MinArmorBoost;
        [SerializeField] private float _armorBoostRandomDeviation = 0f;

        [SerializeField] private float _projectilesSlowdown = PlasmaShield.MinProjectilesSlowdown;
        [SerializeField] private float _projectilesSlowdownRandomDeviation = 0f;

        public RangedFloat ArmorBoost { get; private set; }
        public RangedFloat ProjectilesSlowdown { get; private set; }

        public override void ApplySettings()
        {
            base.ApplySettings();

            ArmorBoost = new(_armorBoost, _armorBoostRandomDeviation, PlasmaShield.MinArmorBoost, PlasmaShield.MaxArmorBoost);
            ProjectilesSlowdown = new(_projectilesSlowdown, _projectilesSlowdownRandomDeviation, PlasmaShield.MinProjectilesSlowdown, PlasmaShield.MaxProjectilesSlowdown);
        }

        public override InventoryItem GetItem() => new PlasmaShield(Rarity,
                                                                    Duration.RandomValue,
                                                                    SellValue.RandomValue,
                                                                    ArmorBoost.RandomValue,
                                                                    ProjectilesSlowdown.RandomValue);
    }
}