using SpaceAce.Gameplay.Inventories;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceAce.UI
{
    public sealed class ItemInfusionSlot
    {
        public event EventHandler<ItemEventArgs> ItemRemoved;

        private readonly VisualElement _itemIcon;
        private readonly Button _removeItemButton;

        public InventoryItem Item { get; private set; } = null;
        public bool IsEmpty => Item == null;

        public ItemInfusionSlot(VisualElement infusionSlot)
        {
            if (infusionSlot == null) throw new ArgumentNullException(nameof(infusionSlot));

            _itemIcon = infusionSlot.Q<VisualElement>("Item-icon");

            _removeItemButton = infusionSlot.Q<Button>("Remove-item-button");
            _removeItemButton.SetEnabled(false);
        }

        public void OnEnable()
        {
            _removeItemButton.clicked += RemoveItemButtonClickedEventHandler;
        }

        public void OnDisable()
        {
            _removeItemButton.clicked -= RemoveItemButtonClickedEventHandler;
        }

        public bool AddItem(InventoryItem item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));

            if (IsEmpty == false) return false;

            Item = item;

            _itemIcon.style.backgroundColor = new(item.RarityColor);
            _itemIcon.style.backgroundImage = new(item.Icon);

            _removeItemButton.SetEnabled(true);

            return true;
        }

        public void Clear()
        {
            if (IsEmpty) return;

            Item = null;

            _itemIcon.style.backgroundColor = new Color(0f, 0f, 0f, 0f);
            _itemIcon.style.backgroundImage = null;

            _removeItemButton.SetEnabled(false);
        }

        private void RemoveItemButtonClickedEventHandler()
        {
            ItemRemoved?.Invoke(this, new(Item));

            Item = null;

            _itemIcon.style.backgroundColor = new Color(0f, 0f, 0f, 0f);
            _itemIcon.style.backgroundImage = null;

            _removeItemButton.SetEnabled(false);
        }
    }
}