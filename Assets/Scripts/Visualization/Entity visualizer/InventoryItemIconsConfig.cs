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
        [SerializeField] private Sprite _atomizerIcon;
        [SerializeField] private Sprite _massNegatorIcon;
        [SerializeField] private Sprite _matterDegausserIcon;

        public Sprite GetIcon(Type itemType)
        {
            if (itemType is null) throw new ArgumentNullException(nameof(itemType), "Attempted to pass an empty inventory item type!");

            if (itemType == typeof(PlasmaShield)) return _plasmaShieldIcon;

            return null;
        }
    }
}