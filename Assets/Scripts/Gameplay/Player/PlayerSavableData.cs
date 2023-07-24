using SpaceAce.Gameplay.Inventories;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Gameplay.Players
{
    [Serializable]
    public sealed class PlayerSavableData
    {
        [SerializeField] private string _selectedShipAnchorName;
        [SerializeField] private List<InventoryItem> _inventoryContent = new();
        [SerializeField] private int _credits;
        [SerializeField] private float _experience;

        public string SelectedShipAnchorName => _selectedShipAnchorName;
        public IEnumerable<InventoryItem> InventoryContent => _inventoryContent;
        public int Credits => _credits;
        public float Experience => _experience;

        public PlayerSavableData(string selectedShipAnchorName,
                                 IEnumerable<InventoryItem> inventoryItems,
                                 int credits,
                                 float experience)
        {
            if (string.IsNullOrEmpty(selectedShipAnchorName) ||
                string.IsNullOrWhiteSpace(selectedShipAnchorName)) throw new ArgumentNullException(nameof(selectedShipAnchorName));

            if (inventoryItems is not null) _inventoryContent = new(inventoryItems);

            _selectedShipAnchorName = selectedShipAnchorName;
            _credits = credits;
            _experience = experience;
        }
    }
}