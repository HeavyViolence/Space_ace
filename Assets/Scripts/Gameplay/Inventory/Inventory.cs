using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    public sealed class Inventory
    {
        public const int DefaultCapacity = 100;

        public event EventHandler ContentChanged;

        [SerializeField] private HashSet<InventoryItem> _items = new(DefaultCapacity);

        public int ItemsAmount => _items.Count;

        public Inventory() { }

        public IEnumerable<InventoryItem> GetContent() => _items;

        public void AddItem(InventoryItem item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));

            _items.Add(item);
            ContentChanged?.Invoke(this, EventArgs.Empty);
        }

        public void AddItems(IEnumerable<InventoryItem> items)
        {
            if (items is null) throw new ArgumentNullException(nameof(items));

            foreach (var item in items) _items.Add(item);

            ContentChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool RemoveItem(InventoryItem item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));

            if (_items.Remove(item) == true)
            {
                ContentChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }

            return false;
        }

        public void Clear()
        {
            _items.Clear();
            ContentChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}