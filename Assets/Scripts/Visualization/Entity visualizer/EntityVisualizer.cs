using SpaceAce.Architecture;
using SpaceAce.Gameplay.Inventories;
using System;
using UnityEngine;

namespace SpaceAce.Visualization
{
    public sealed class EntityVisualizer : IGameService
    {
        private InventoryItemIconsConfig _inventoryItemIconsConfig;
        private InventoryItemRarityColorsConfig _inventoryItemRarityColorsConfig;

        public EntityVisualizer(InventoryItemIconsConfig iconsConfig,
                                InventoryItemRarityColorsConfig rarityColorsConfig)
        {
            if (iconsConfig == null) throw new ArgumentNullException(nameof(iconsConfig));
            if (rarityColorsConfig == null) throw new ArgumentNullException(nameof(rarityColorsConfig));

            _inventoryItemIconsConfig = iconsConfig;
            _inventoryItemRarityColorsConfig = rarityColorsConfig;
        }

        public Sprite GetInventoryItemIcon(Type type) => _inventoryItemIconsConfig.GetIcon(type);

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