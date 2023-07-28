using SpaceAce.Gameplay.Inventories;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace SpaceAce.UI
{
    public sealed class ItemInfuser
    {
        public event EventHandler<ItemEventArgs> ItemToInfuseCollected;
        public event EventHandler<ItemEventArgs> InfusedItemCollected;

        private readonly List<ItemToInfuseSlot> _itemInfusionSlots = new(InventoryItem.ItemsPerInfusion);
        private InfusedItemSlot _infusedItemSlot;
        private Button _infuseButton;

        public bool InfusionSlotsPopulated
        {
            get
            {
                if (_itemInfusionSlots.Count == 0) return false;

                foreach (var slot in _itemInfusionSlots) if (slot.IsEmpty) return false;

                return true;
            }
        }

        public ItemInfuser() { }

        public void SetInventoryDisplay(VisualElement inventory)
        {
            if (inventory == null) throw new ArgumentNullException(nameof(inventory));

            _itemInfusionSlots.Clear();

            for (int i = 0; i < InventoryItem.ItemsPerInfusion; i++)
            {
                var slot = inventory.Q<VisualElement>($"Item-infusion-slot-{i}");
                ItemToInfuseSlot infusionSlot = new(slot);

                _itemInfusionSlots.Add(infusionSlot);
            }

            var infusedItemSlot = inventory.Q<VisualElement>("Infused-item-slot");
            _infusedItemSlot = new(infusedItemSlot);

            _infuseButton = inventory.Q<Button>("Infuse-button");
            _infuseButton.SetEnabled(false);
            _infuseButton.clicked += InfuseButtonClickedEventHandler;
        }

        public void OnEnable()
        {
            foreach (var slot in  _itemInfusionSlots) slot.ItemRemoved += ItemToInfuseCollectedEventHandler;
            _infusedItemSlot.ItemCollected += InfusedItemCollectedEventHandler;

            foreach (var slot in _itemInfusionSlots) slot.OnEnable();
            _infusedItemSlot.OnEnable();
        }

        public void OnDisable()
        {
            foreach (var slot in _itemInfusionSlots) slot.ItemRemoved -= ItemToInfuseCollectedEventHandler;
            _infusedItemSlot.ItemCollected -= InfusedItemCollectedEventHandler;

            foreach (var slot in _itemInfusionSlots) slot.OnDisable();
            _infusedItemSlot.OnDisable();
        }

        public bool AddItem(InventoryItem item)
        {
            foreach (var slot in _itemInfusionSlots)
            {
                if (slot.AddItem(item) == true)
                {
                    if (InfusionSlotsPopulated && _infusedItemSlot.IsEmpty) _infuseButton?.SetEnabled(true);

                    return true;
                }
            }

            return false;
        }

        public bool TryReclaimItemsToInfuse(out IEnumerable<InventoryItem> items)
        {
            List<InventoryItem> itemsToCollect = new(InventoryItem.ItemsPerInfusion);

            foreach (var slot in _itemInfusionSlots)
            {
                if (slot.IsEmpty == false)
                {
                    itemsToCollect.Add(slot.Item);
                    slot.Clear();
                }
            }

            if (itemsToCollect.Count > 0)
            {
                items = itemsToCollect;
                return true;
            }
            else
            {
                items = null;
                return false;
            }
        }

        public bool TryCollectInfusedItem(out InventoryItem item)
        {
            if (_infusedItemSlot.IsEmpty == false)
            {
                item = _infusedItemSlot.Item;
                _infusedItemSlot.Clear();

                return true;
            }
            else
            {
                item = null;
                return false;
            }
        }

        #region event handlers

        private void InfuseButtonClickedEventHandler()
        {
            var item1 = _itemInfusionSlots[0].Item;
            var item2 = _itemInfusionSlots[1].Item;
            var item3 = _itemInfusionSlots[2].Item;

            if (item1.Fuse(item2, item3, out var infusedItem) == true)
            {
                _infusedItemSlot.AddItem(infusedItem);

                foreach (var slot in _itemInfusionSlots) slot.Clear();

                _infuseButton.SetEnabled(false);
            }
        }

        private void ItemToInfuseCollectedEventHandler(object sender, ItemEventArgs e)
        {
            ItemToInfuseCollected?.Invoke(this, e);
        }

        private void InfusedItemCollectedEventHandler(object sender, ItemEventArgs e)
        {
            InfusedItemCollected?.Invoke(this, e);

            if (InfusionSlotsPopulated) _infuseButton.SetEnabled(true);
        }

        #endregion
    }
}