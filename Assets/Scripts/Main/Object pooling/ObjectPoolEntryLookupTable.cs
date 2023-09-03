using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Main.ObjectPooling
{
    [CreateAssetMenu(fileName = "Object pool entry lookup table", menuName = "Space ace/Object pooling/Object pool entry lookup table")]
    public sealed class ObjectPoolEntryLookupTable : ScriptableObject
    {
        [SerializeField] private List<string> _keys = new();
        [SerializeField] private List<ObjectPoolEntry> _values = new();

        public int EntriesAmount => _keys.Count;

        public void AddEntry(ObjectPoolEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            if (_values.Contains(entry) == false)
            {
                _keys.Add(Guid.NewGuid().ToString());
                _values.Add(entry);
            }
        }

        public void AddSpecificEntry(string id, ObjectPoolEntry entry)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrWhiteSpace(id) || entry == null) throw new ArgumentNullException(nameof(entry));

            if (_values.Contains(entry) == false)
            {
                _keys.Add(id);
                _values.Add(entry);
            }
        }

        public void AddEntries(IEnumerable<ObjectPoolEntry> entries)
        {
            if (entries is null) throw new ArgumentNullException(nameof(entries));

            foreach (var entry in entries) AddEntry(entry);
        }

        public void RemoveEntry(ObjectPoolEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            if (_values.Contains(entry))
            {
                int keyIndex = _values.IndexOf(entry);

                _keys.RemoveAt(keyIndex);
                _values.Remove(entry);
            }
        }

        public void Clear()
        {
            _keys.Clear();
            _values.Clear();
        }

        public bool TryGetEntry(string id, out ObjectPoolEntry entry)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
            {
                entry = default;
                return false;
            }

            int index = _keys.IndexOf(id);

            if (index != -1)
            {
                entry = _values[index];
                return true;
            }

            entry = default;
            return false;
        }

        public bool TryGetEntryByName(string anchorName, out ObjectPoolEntry entry)
        {
            if (string.IsNullOrEmpty(anchorName) || string.IsNullOrWhiteSpace(anchorName))
            {
                entry = default;
                return false;
            }

            foreach (var value in _values)
            {
                if (value.AnchorName.Equals(anchorName))
                {
                    entry = value;
                    return true;
                }
            }

            entry = default;
            return false;
        }

        public IEnumerable<KeyValuePair<string, ObjectPoolEntry>> GetContents()
        {
            Dictionary<string, ObjectPoolEntry> result = new(EntriesAmount);

            for (int i = 0; i < EntriesAmount; i++) result.Add(_keys[i], _values[i]);

            return result;
        }
    }
}