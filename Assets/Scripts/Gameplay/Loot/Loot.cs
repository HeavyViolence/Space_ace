using SpaceAce.Architecture;
using SpaceAce.Gameplay.Amplifications;
using SpaceAce.Gameplay.Damageables;
using SpaceAce.Gameplay.Inventories;
using SpaceAce.Gameplay.Spawning;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Gameplay.Loot
{
    public abstract class Loot : MonoBehaviour
    {
        private static readonly GameServiceFastAccess<LootSpawner> s_lootSpawner = new();
        private static readonly GameServiceFastAccess<SpecialEffectsMediator> s_specialEffectsMediator = new();
        protected static readonly GameServiceFastAccess<GamePauser> GamePauser = new();

        [SerializeField] private LootConfig _lootConfig;

        [SerializeField] private bool _enableAmplifiedLoot = false;
        [SerializeField] private LootConfig _amplifiedLootConfig;

        private IDestroyable _destroyable;
        private Amplifier _amplifier;

        protected float SpawnProbabilityIncrease = 0f;

        protected virtual void Awake()
        {
            if (transform.TryGetComponent(out IDestroyable destroyable) == true) _destroyable = destroyable;
            else throw new MissingComponentException();

            if (_enableAmplifiedLoot && transform.TryGetComponent(out Amplifier amplifier) == true) _amplifier = amplifier;
        }

        protected virtual void OnEnable()
        {
            _destroyable.Destroyed += DestroyedEventHandler;
            s_specialEffectsMediator.Access.Register(this);
        }

        protected virtual void OnDisable()
        {
            _destroyable.Destroyed -= DestroyedEventHandler;
            s_specialEffectsMediator.Access.Deregister(this);

            SpawnProbabilityIncrease = 0f;
        }

        private void DestroyedEventHandler(object sender, DestroyedEventArgs e)
        {
            if (_amplifier != null &&
                _amplifier.Active &&
                _amplifiedLootConfig.GetLootIfProbable(out IEnumerable<InventoryItem> amplifiedLoot, SpawnProbabilityIncrease) == true)
            {
                s_lootSpawner.Access.SpawnLoot(amplifiedLoot, e.DeathPosition);
            }
            else if (_lootConfig.GetLootIfProbable(out IEnumerable<InventoryItem> loot) == true)
            {
                s_lootSpawner.Access.SpawnLoot(loot, e.DeathPosition);
            }
        }
    }
}