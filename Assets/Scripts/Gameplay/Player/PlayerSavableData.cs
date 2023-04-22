using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Players
{
    [Serializable]
    public sealed class PlayerSavableData
    {
        [SerializeField] private string _selectedShipAnchorName;

        public string SelectedShipAnchorName => _selectedShipAnchorName;

        public PlayerSavableData(string selectedShipAnchorName)
        {
            if (string.IsNullOrEmpty(selectedShipAnchorName) ||
                string.IsNullOrWhiteSpace(selectedShipAnchorName))
            {
                throw new ArgumentNullException(nameof(selectedShipAnchorName), "Attepted to pass an empty selected ship anchor name!");
            }

            _selectedShipAnchorName = selectedShipAnchorName;
        }
    }
}