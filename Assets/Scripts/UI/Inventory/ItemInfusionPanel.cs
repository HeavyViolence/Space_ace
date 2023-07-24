using SpaceAce.Gameplay.Inventories;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace SpaceAce.UI
{
    public sealed class ItemInfusionPanel
    {
        public event EventHandler<ItemEventArgs> ItemToInfuseCollected;
        public event EventHandler<ItemEventArgs> InfusedItemCollected;

        private readonly List<ItemInfusionSlot> _itemInfusionSlots = new(InventoryItem.ItemsPerInfusion);
        private readonly InfusedItemSlot _infusedItemSlot;
        private readonly Button _infuseButton;

        public bool InfusionSlotsPopulated => _itemInfusionSlots.Count == InventoryItem.ItemsPerInfusion;

        public ItemInfusionPanel(VisualElement inventoryDisplay)
        {
            if (inventoryDisplay == null) throw new ArgumentNullException(nameof(inventoryDisplay));

            for (int i = 0; i < InventoryItem.ItemsPerInfusion; i++)
            {
                var slot = inventoryDisplay.Q<VisualElement>($"Item-infusion-slot-{i}");
                ItemInfusionSlot infusionSlot = new(slot);

                _itemInfusionSlots.Add(infusionSlot);
            }

            var infusedItemSlot = inventoryDisplay.Q<VisualElement>("Infused-item-slot");
            _infusedItemSlot = new(infusedItemSlot);

            _infuseButton = inventoryDisplay.Q<Button>("Infuse-button");
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
            if (InfusionSlotsPopulated) return false;

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
    }
}