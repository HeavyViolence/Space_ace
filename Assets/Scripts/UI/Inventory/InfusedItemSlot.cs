using SpaceAce.Gameplay.Inventories;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceAce.UI
{
    public sealed class InfusedItemSlot
    {
        public event EventHandler<ItemEventArgs> ItemCollected;

        private readonly VisualElement _itemIcon;
        private readonly Button _collectItemButton;

        public InventoryItem Item { get; private set; } = null;
        public bool IsEmpty => Item == null;

        public InfusedItemSlot(VisualElement infusedItemSlot)
        {
            if (infusedItemSlot == null) throw new ArgumentNullException(nameof(infusedItemSlot));

            _itemIcon = infusedItemSlot.Q<VisualElement>("Item-icon");

            _collectItemButton = infusedItemSlot.Q<Button>("Collect-item-button");
            _collectItemButton.SetEnabled(false);
        }

        public void OnEnable()
        {
            _collectItemButton.clicked += CollectItemButtonClickedEventHandler;
        }

        public void OnDisable()
        {
            _collectItemButton.clicked -= CollectItemButtonClickedEventHandler;
        }

        public bool AddItem(InventoryItem item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));

            if (IsEmpty == false) return false;

            Item = item;

            _itemIcon.style.backgroundColor = new(item.RarityColor);
            _itemIcon.style.backgroundImage = new(item.Icon);

            _collectItemButton.SetEnabled(true);

            return true;
        }

        private void CollectItemButtonClickedEventHandler()
        {
            ItemCollected?.Invoke(this, new(Item));

            Item = null;

            _itemIcon.style.backgroundColor = new Color(0f, 0f, 0f, 0f);
            _itemIcon.style.backgroundImage = null;

            _collectItemButton.SetEnabled(false);
        }
    }
}