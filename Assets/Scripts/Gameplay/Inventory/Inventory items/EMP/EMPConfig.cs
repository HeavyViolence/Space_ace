using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    [CreateAssetMenu(fileName = "EMP", menuName = "Space ace/Configs/Loot/EMP")]
    public sealed class EMPConfig : InventoryItemConfig
    {
        [SerializeField] private float _jamProbability = EMP.MinJamProbability;
        [SerializeField] private float _jamProbabilityRandomDeviation = 0f;

        public RangedFloat JamProbability { get; private set; }

        public override void ApplySettings()
        {
            base.ApplySettings();

            JamProbability = new(_jamProbability, _jamProbabilityRandomDeviation, EMP.MinJamProbability, EMP.MaxJamProbability);
        }

        public override InventoryItem GetItem() => new EMP(Rarity, Duration.RandomValue, JamProbability.RandomValue);
    }
}