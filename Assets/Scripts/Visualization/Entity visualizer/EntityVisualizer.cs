using SpaceAce.Architecture;
using SpaceAce.Gameplay.Inventory;
using System;
using UnityEngine;

namespace SpaceAce.Visualization
{
    public sealed class EntityVisualizer : IGameService
    {
        private InventoryItemIconsConfig _inventoryItemIconsConfig;
        private InventoryItemRarityColorConfig _inventoryItemRarityColorsConfig;

        public EntityVisualizer(InventoryItemIconsConfig iconsConfig,
                                InventoryItemRarityColorConfig rarityColorsConfig)
        {
            if (iconsConfig == null) throw new ArgumentNullException(nameof(iconsConfig));
            if (rarityColorsConfig == null) throw new ArgumentNullException(nameof(rarityColorsConfig));

            _inventoryItemIconsConfig = iconsConfig;
            _inventoryItemRarityColorsConfig = rarityColorsConfig;
        }

        public Sprite GetInventoryItemIcon(string itemType) => _inventoryItemIconsConfig.GetIcon(itemType);

        public Color32 GetInventoryItemRarityColor(ItemRarity rarity) => _inventoryItemRarityColorsConfig.GetRarityColor(rarity);

        #region interfaces

        public void OnInitialize()
        {
            GameServices.Register(this);
        }

        public void OnSubscribe()
        {

        }

        public void OnUnsubscribe()
        {

        }

        public void OnClear()
        {
            GameServices.Deregister(this);
        }

        #endregion
    }
}