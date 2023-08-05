using SpaceAce.Gameplay.Inventories;
using System;
using UnityEngine;

namespace SpaceAce.Visualization
{
    [CreateAssetMenu(fileName = "Inventory item icons config",
                     menuName = "Space ace/Configs/Inventory/Item icons config")]
    public sealed class InventoryItemIconsConfig : ScriptableObject
    {
        [SerializeField] private Sprite _plasmaShieldIcon;
        [SerializeField] private Sprite _repairKitIcon;
        [SerializeField] private Sprite _armorDiffuserIcon;
        [SerializeField] private Sprite _reactiveArmorIcon;
        [SerializeField] private Sprite _nanofuelIcon;
        [SerializeField] private Sprite _stasisFieldIcon;
        [SerializeField] private Sprite _homingAmmoIcon;

        public Sprite GetIcon(Type itemType)
        {
            if (itemType is null) throw new ArgumentNullException(nameof(itemType));

            if (itemType == typeof(PlasmaShield)) return _plasmaShieldIcon;
            if (itemType == typeof(RepairKit)) return _repairKitIcon;
            if (itemType == typeof(ArmorDiffuser)) return _armorDiffuserIcon;
            if (itemType == typeof(ReactiveArmor)) return _reactiveArmorIcon;
            if (itemType == typeof(Nanofuel)) return _nanofuelIcon;
            if (itemType == typeof(StasisField)) return _stasisFieldIcon;
            if (itemType == typeof(HomingAmmo)) return _homingAmmoIcon;

            return null;
        }
    }
}