using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    [CreateAssetMenu(fileName = "Stasis field", menuName = "Space ace/Configs/Loot/Stasis field")]
    public sealed class StasisFieldConfig : InventoryItemConfig
    {
        [SerializeField] private float _slowdown = StasisField.MinSlowdown;
        [SerializeField] private float _slowdownRandomDeviation = 0f;

        public RangedFloat Slowdown { get; private set; }

        public override void ApplySettings()
        {
            base.ApplySettings();

            Slowdown = new(_slowdown, _slowdownRandomDeviation, StasisField.MinSlowdown, StasisField.MaxSlowdown);
        }

        public override InventoryItem GetItem() => new StasisField(Rarity, Duration.RandomValue, Slowdown.RandomValue);
    }
}