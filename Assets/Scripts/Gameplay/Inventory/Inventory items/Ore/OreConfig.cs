using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    [CreateAssetMenu(fileName = "Ore", menuName = "Space ace/Configs/Loot/Ore")]
    public sealed class OreConfig : InventoryItemConfig
    {
        [SerializeField] private float _density = Ore.MinDensity;
        [SerializeField] private float _densityRandomDeviation = 0f;

        public RangedFloat Density { get; private set; }

        public override void ApplySettings()
        {
            base.ApplySettings();

            Density = new(_density, _densityRandomDeviation, Ore.MinDensity, Ore.MaxDensity);
        }

        public override InventoryItem GetItem() => new Ore(Rarity, Density.RandomValue);
    }
}