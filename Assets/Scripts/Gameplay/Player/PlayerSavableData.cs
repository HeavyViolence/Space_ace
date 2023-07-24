using SpaceAce.Gameplay.Inventories;
using System;
using System.Collections.Generic;

namespace SpaceAce.Gameplay.Players
{
    public sealed class PlayerSavableData
    {
        public string SelectedShipAnchorName { get; private set; }
        public List<InventoryItem> InventoryContent { get; private set; } = new();
        public int Credits { get; private set; }
        public float Experience { get; private set; }

        public PlayerSavableData(string selectedShipAnchorName,
                                 IEnumerable<InventoryItem> inventoryItems,
                                 int credits,
                                 float experience)
        {
            if (string.IsNullOrEmpty(selectedShipAnchorName) ||
                string.IsNullOrWhiteSpace(selectedShipAnchorName)) throw new ArgumentNullException(nameof(selectedShipAnchorName));

            if (inventoryItems is not null) InventoryContent = new(inventoryItems);

            SelectedShipAnchorName = selectedShipAnchorName;
            Credits = credits;
            Experience = experience;
        }
    }
}