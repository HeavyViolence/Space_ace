using SpaceAce.Architecture;
using SpaceAce.Auxiliary;
using SpaceAce.Gameplay.Inventory;
using SpaceAce.Gameplay.Loot;
using SpaceAce.Gameplay.Movement;
using SpaceAce.Main;
using SpaceAce.Main.ObjectPooling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Gameplay.Spawning
{
    public sealed class LootSpawner : IGameService, IRunnable
    {
        private const float SpawnPositionMaxRandomDeviation = 5f;
        private const float EscapePositionDelta = 5f;

        private static readonly GameServiceFastAccess<MultiobjectPool> _multiobjectPool = new();
        private static readonly GameServiceFastAccess<MasterCameraHolder> _masterCameraHolder = new();

        private ObjectPoolEntry _lootItemBox;

        public LootSpawner(ObjectPoolEntry lootItemBox)
        {
            if (lootItemBox == null) throw new ArgumentNullException(nameof(lootItemBox), "Attempted to pass an empty loot item box!");

            _lootItemBox = lootItemBox;
        }

        public void SpawnLoot(IEnumerable<InventoryItem> loot, Vector2 position)
        {
            foreach (var item in loot)
            {
                GameObject lootItemBox = _multiobjectPool.Access.GetObject(_lootItemBox.AnchorName);
                lootItemBox.transform.position = GetSpawnPosition(position);

                if (lootItemBox.TryGetComponent(out LootItem lootItem) == true)
                {
                    lootItem.SetContent(item);
                    lootItem.Collected += (s, e) => _multiobjectPool.Access.ReleaseObject(_lootItemBox.AnchorName, lootItemBox, () => true);
                }
                else
                {
                    throw new MissingComponentException($"Spawned loot item box {lootItemBox.name} doesn't have a mandatory component of type {typeof(LootItem)}!");
                }

                if (lootItemBox.TryGetComponent(out IEscapable escapable) == true)
                {
                    escapable.StartWatchingForEscape(() => _masterCameraHolder.Access.InsideViewport(lootItemBox.transform.position, EscapePositionDelta));
                    escapable.Escaped += (s, e) => _multiobjectPool.Access.ReleaseObject(_lootItemBox.AnchorName, lootItemBox, () => true);
                }
                else
                {
                    throw new MissingComponentException($"Spawned loot item box {lootItemBox.name} doesn't have a mandatory component of type {typeof(IEscapable)}!");
                }
            }
        }

        private Vector2 GetSpawnPosition(Vector2 pivotPosition)
        {
            float rawX = pivotPosition.x + AuxMath.RandomNormal * SpawnPositionMaxRandomDeviation;
            float rawY = pivotPosition.y + AuxMath.RandomNormal * SpawnPositionMaxRandomDeviation;

            float clampedX = Mathf.Clamp(rawX, _masterCameraHolder.Access.ViewportLeftBound, _masterCameraHolder.Access.ViewportRightBound);
            float clampexY = Mathf.Clamp(rawY, _masterCameraHolder.Access.ViewportLowerBound, _masterCameraHolder.Access.ViewportUpperBound);

            return new(clampedX, clampexY);
        }

        #region interfaces

        public void OnInitialize()
        {
            GameServices.Register(this);
        }

        public void OnSubscribe()
        {

        }

        public void OnUnsubscribe()
        {

        }

        public void OnClear()
        {
            GameServices.Deregister(this);
        }

        public void OnRun()
        {
            _lootItemBox.EnsureObjectPoolExistence();
        }

        #endregion
    }
}