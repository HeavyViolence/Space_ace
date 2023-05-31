using SpaceAce.Architecture;
using SpaceAce.Gameplay.Amplifications;
using SpaceAce.Gameplay.Damageables;
using SpaceAce.Gameplay.Inventory;
using SpaceAce.Gameplay.Spawning;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Gameplay.Loot
{
    public sealed class Loot : MonoBehaviour
    {
        private static readonly GameServiceFastAccess<LootSpawner> _lootSpawner = new();

        [SerializeField] private LootConfig _lootConfig;

        [SerializeField] private bool _enableAmplifiedLoot = false;
        [SerializeField] private LootConfig _amplifiedLootConfig;

        private IDestroyable _destroyable;
        private Amplifier _amplifier;

        private void Awake()
        {
            if (transform.TryGetComponent(out IDestroyable destroyable) == true)
            {
                _destroyable = destroyable;
            }
            else
            {
                throw new MissingComponentException($"Object {name} is missing a mandatory component of type {typeof(IDestroyable)}!");
            }

            if (_enableAmplifiedLoot && transform.TryGetComponent(out Amplifier amplifier) == true)
            {
                _amplifier = amplifier;
            }
        }

        private void OnEnable()
        {
            _destroyable.Destroyed += DestroyedEventHandler;
        }

        private void OnDisable()
        {
            _destroyable.Destroyed -= DestroyedEventHandler;
        }

        private void DestroyedEventHandler(object sender, DestroyedEventArgs e)
        {
            if (_amplifier != null &&
                _amplifier.Active &&
                _amplifiedLootConfig.GetLootIfProbable(out IEnumerable<InventoryItem> amplifiedLoot) == true)
            {
                _lootSpawner.Access.SpawnLoot(amplifiedLoot, e.DeathPosition);
            }
            else if (_lootConfig.GetLootIfProbable(out IEnumerable<InventoryItem> loot) == true)
            {
                _lootSpawner.Access.SpawnLoot(loot, e.DeathPosition);
            }
        }
    }
}