using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventory
{
    public interface IInventoryItem : IEquatable<IInventoryItem>
    {
        string ItemType { get; }
        ItemRarity Rarity { get; }
        Color32 RarityColor { get; }
        Sprite Icon { get; }
        string Title { get; }
        string Description { get; }
        string Stats { get; }

        void Use();
        bool Fuse(IInventoryItem pair, out IInventoryItem result);
        int Scrap();
    }
}