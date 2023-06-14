using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    [CreateAssetMenu(fileName = "Plasma shield", menuName = "Space ace/Configs/Loot/Plasma shield")]
    public sealed class PlasmaShieldConfig : InventoryItemConfig
    {
        public const float MinArmorBoost = 100f;
        public const float MaxArmorBoost = 10000f;

        [SerializeField] private float _armorBoost = MinArmorBoost;
        [SerializeField] private float _armorBoostRandomDeviation = 0f;

        public RangedFloat ArmorBoost { get; private set; }

        public override void ApplySettings()
        {
            base.ApplySettings();

            ArmorBoost = new(_armorBoost, _armorBoostRandomDeviation);
        }

        public override InventoryItem GetItem() => new PlasmaShield(Rarity,
                                                                    Duration.RandomValue,
                                                                    ScrapValue.RandomValue,
                                                                    ArmorBoost.RandomValue);
    }
}