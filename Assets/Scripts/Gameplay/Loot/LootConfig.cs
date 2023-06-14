using SpaceAce.Auxiliary;
using SpaceAce.Gameplay.Inventories;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Gameplay.Loot
{
    [CreateAssetMenu(fileName = "Loot config", menuName = "Space ace/Configs/Loot/Loot config")]
    public sealed class LootConfig : ScriptableObject
    {
        public const int MinLootAmount = 1;
        public const int MaxLootAmount = 10;

        [SerializeField] private List<InventoryItemConfig> _loot;

        [SerializeField] private int _lootAmount = MinLootAmount;
        [SerializeField] private int _lootAmountRandomDeviation = 0;

        [SerializeField] private float _spawnProbability = 0f;

        private readonly Dictionary<ItemRarity, List<InventoryItemConfig>> _sortedLootGroups = new();

        public RangedInt LootAmount { get; private set; }

        private void OnEnable()
        {
            SortLoot();
            ApplySettings();
        }

        private void SortLoot()
        {
            if (_loot.Count > 0)
            {
                _loot.Sort(CompareItemConfigsByRarityDescending);

                foreach (var itemConfig in _loot)
                {
                    if (_sortedLootGroups.TryGetValue(itemConfig.Rarity, out var lootGroupOfTheSameRarity) == true)
                    {
                        lootGroupOfTheSameRarity.Add(itemConfig);
                    }
                    else
                    {
                        _sortedLootGroups.Add(itemConfig.Rarity, new List<InventoryItemConfig>() { itemConfig });
                    }
                }
            }
        }

        private int CompareItemConfigsByRarityDescending(InventoryItemConfig c1, InventoryItemConfig c2)
        {
            if ((int)c1.Rarity < (int)c2.Rarity) return 1;
            if ((int)c1.Rarity > (int)c2.Rarity) return -1;

            return 0;
        }

        public void ApplySettings()
        {
            LootAmount = new(_lootAmount, _lootAmountRandomDeviation, MinLootAmount, MaxLootAmount * 2);
        }

        public bool GetLootIfProbable(out IEnumerable<InventoryItem> loot)
        {
            if (AuxMath.Random < _spawnProbability)
            {
                List<InventoryItem> lootToSpawn = new(LootAmount.RandomValue);

                for (int i = 0; i < lootToSpawn.Capacity; i++)
                {
                    float seed = AuxMath.Random;

                    foreach (var lootGroup in _sortedLootGroups)
                    {
                        float spawnProbability = InventoryItem.GetHighestSpawnProbabilityFromRarity(lootGroup.Value[0].Rarity);

                        if (seed < spawnProbability)
                        {
                            int lootConfigIndex = Random.Range(0, lootGroup.Value.Count);
                            InventoryItem item = lootGroup.Value[lootConfigIndex].GetItem();

                            lootToSpawn.Add(item);

                            break;
                        }
                    }
                }

                loot = lootToSpawn;
                return true;
            }
            else
            {
                loot = null;
                return false;
            }
        }
    }
}