using SpaceAce.Gameplay.Inventories;
using UnityEngine;

namespace SpaceAce.Visualization
{
    [CreateAssetMenu(fileName = "Inventory item rarity color config",
                     menuName = "Space ace/Configs/Inventory/Item rarity color config")]
    public sealed class InventoryItemRarityColorsConfig : ScriptableObject
    {
        [SerializeField] private Color32 _commonColor;
        [SerializeField] private Color32 _uncommonColor;
        [SerializeField] private Color32 _rareColor;
        [SerializeField] private Color32 _exceptionalColor;
        [SerializeField] private Color32 _exoticColor;
        [SerializeField] private Color32 _epicColor;
        [SerializeField] private Color32 _legendaryColor;

        public Color32 GetRarityColor(ItemRarity rarity)
        {
            return rarity switch
            {
                ItemRarity.Common => _commonColor,
                ItemRarity.Uncommon => _uncommonColor,
                ItemRarity.Rare => _rareColor,
                ItemRarity.Exceptional => _exceptionalColor,
                ItemRarity.Exotic => _exoticColor,
                ItemRarity.Epic => _epicColor,
                ItemRarity.Legendary => _legendaryColor,
                _ => new(0, 0, 0, 0)
            };
        }
    }
}