using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    [CreateAssetMenu(fileName = "Meteor router", menuName = "Space ace/Configs/Loot/Meteor router")]
    public sealed class MeteorRouterConfig : InventoryItemConfig
    {
        [SerializeField] private float _meteorSpawnSpeedup = MeteorRouter.MinMeteorSpawnSpeedup;
        [SerializeField] private float _meteorSpawnSpeedupRandomDeviation = 0f;

        public RangedFloat MeteorSpawnSpeedup { get; private set; }

        public override void ApplySettings()
        {
            base.ApplySettings();

            MeteorSpawnSpeedup = new(_meteorSpawnSpeedup,
                                     _meteorSpawnSpeedupRandomDeviation,
                                     MeteorRouter.MinMeteorSpawnSpeedup,
                                     MeteorRouter.MaxMeteorSpawnSpeedup);
        }

        public override InventoryItem GetItem() => new MeteorRouter(Rarity,
                                                                    Duration.RandomValue,
                                                                    MeteorSpawnSpeedup.RandomValue);        
    }
}