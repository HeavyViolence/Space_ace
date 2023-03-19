using System;
using System.Runtime.Serialization;

namespace SpaceAce.Gameplay.Players
{
    [DataContract]
    public sealed class PlayerSavableData
    {
        [DataMember]
        public string SelectedShipAnchorName { get; private set; }

        public PlayerSavableData(string selectedShipAnchorName)
        {
            if (string.IsNullOrEmpty(selectedShipAnchorName) ||
                string.IsNullOrWhiteSpace(selectedShipAnchorName))
            {
                throw new ArgumentNullException(nameof(selectedShipAnchorName), "Attepted to pass an empty selected ship anchor name!");
            }

            SelectedShipAnchorName = selectedShipAnchorName;
        }
    }
}