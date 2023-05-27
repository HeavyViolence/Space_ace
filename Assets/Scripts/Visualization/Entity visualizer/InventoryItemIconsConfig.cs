using UnityEngine;

namespace SpaceAce.Visualization
{
    [CreateAssetMenu(fileName = "Inventory item icons config",
                     menuName = "Space ace/Configs/Inventory/Item icons config")]
    public sealed class InventoryItemIconsConfig : ScriptableObject
    {
        [SerializeField] private Sprite _plasmaShieldIcon;
        [SerializeField] private Sprite _matterDegausserIcon;
        [SerializeField] private Sprite _atomizerIcon;

        public Sprite GetIcon(string itemType)
        {
            return itemType switch
            {
                "Plasma shield" => _plasmaShieldIcon,
                "Matter degausser" => _matterDegausserIcon,
                "Atomizer" => _atomizerIcon,
                _ => null,
            };
        }
    }
}