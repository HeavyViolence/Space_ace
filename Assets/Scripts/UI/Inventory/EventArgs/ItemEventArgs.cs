using SpaceAce.Gameplay.Inventories;
using System;

namespace SpaceAce.UI
{
    public sealed class ItemEventArgs : EventArgs
    {
        public InventoryItem Item { get; }

        public ItemEventArgs(InventoryItem item)
        {
            Item = item;
        }
    }
}