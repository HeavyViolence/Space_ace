using SpaceAce.Gameplay.Inventory;
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

        public Sprite GetIcon(Type type)
        {
            if (type.Equals(typeof(PlasmaShield))) return _plasmaShieldIcon;

            return null;
        }
    }
}