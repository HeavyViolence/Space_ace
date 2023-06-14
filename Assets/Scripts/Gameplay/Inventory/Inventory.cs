using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    public sealed class Inventory
    {
        public const int DefaultCapacity = 100;
        public const int InfusionSlotsAmount = 3;

        public event EventHandler ContentChanged, InfusionContentChanged;

        [SerializeField] private List<InventoryItem> _inventoryItems;
        [SerializeField] private List<InventoryItem> _itemsToInfuse;

        public int ItemsAmount => _inventoryItems.Count;
        public int ItemsReadyToInfuse => _itemsToInfuse.Count;

        public Inventory()
        { 
            _inventoryItems = new(DefaultCapacity);
            _itemsToInfuse = new(InfusionSlotsAmount);
        }

        public IEnumerable<InventoryItem> GetContent() => _inventoryItems;

        public void AddItem(InventoryItem item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item), "Attempted to add an empty item to the inventory!");

            _inventoryItems.Add(item);
            ContentChanged?.Invoke(this, EventArgs.Empty);
        }

        public void AddItems(IEnumerable<InventoryItem> items)
        {
            if (items is null) throw new ArgumentNullException(nameof(items), "Attempted to add an empty item collection to the inventory!");

            _inventoryItems.AddRange(items);
            ContentChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool RemoveItem(InventoryItem item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item), "Attempted to remove an empty item from the inventory!");

            if (_inventoryItems.Remove(item) == true)
            {
                ContentChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }

            return false;
        }

        public void Clear()
        {
            _inventoryItems.Clear();
            ContentChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool AddItemToInfuse(InventoryItem item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item), "Attempted to add an empty item to infusion!");

            if (_itemsToInfuse.Count < _itemsToInfuse.Capacity)
            {
                _itemsToInfuse.Add(item);
                InfusionContentChanged?.Invoke(this, EventArgs.Empty);

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveItemToInfuse(InventoryItem item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item), "Attempted to remove an empty item from infusion!");

            if (_itemsToInfuse.Remove(item) == true)
            {
                InfusionContentChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public bool FuseItems()
        {
            if (_itemsToInfuse[0].Fuse(_itemsToInfuse[1], _itemsToInfuse[2], out InventoryItem result))
            {
                _inventoryItems.Add(result);
                _itemsToInfuse.Clear();

                ContentChanged?.Invoke(this, EventArgs.Empty);
                InfusionContentChanged?.Invoke(this, EventArgs.Empty);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}