using System;
using System.Runtime.Serialization;

namespace SpaceAce.Gameplay.Players
{
    [DataContract]
    public sealed class PlayerSavableData
    {
        [DataMember]
        public string SelectedPlayerShipAnchorName { get; private set; }

        public PlayerSavableData(string selectedPlayerShipAnchorName)
        {
            if (string.IsNullOrEmpty(selectedPlayerShipAnchorName) ||
                string.IsNullOrWhiteSpace(selectedPlayerShipAnchorName))
            {
                throw new ArgumentNullException(nameof(selectedPlayerShipAnchorName),
                                                "Attepted to pass an empty selected player ship anchor name!");
            }

            SelectedPlayerShipAnchorName = selectedPlayerShipAnchorName;
        }
    }
}