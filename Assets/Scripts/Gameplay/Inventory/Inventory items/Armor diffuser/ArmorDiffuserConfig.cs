using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    [CreateAssetMenu(fileName = "Armor diffuser", menuName = "Space ace/Configs/Loot/Armor diffuser")]
    public sealed class ArmorDiffuserConfig : InventoryItemConfig
    {
        [SerializeField] private float _armorReduction = ArmorDiffuser.MinArmorReduction;
        [SerializeField] private float _armorReductionRandomDeviation = 0f;

        public RangedFloat ArmorReduction { get; private set; }

        public override void ApplySettings()
        {
            base.ApplySettings();

            ArmorReduction = new(_armorReduction, _armorReductionRandomDeviation, ArmorDiffuser.MinArmorReduction, ArmorDiffuser.MaxArmorReduction);
        }

        public override InventoryItem GetItem() => new ArmorDiffuser(Rarity, Duration.RandomValue, ArmorReduction.RandomValue);
    }
}