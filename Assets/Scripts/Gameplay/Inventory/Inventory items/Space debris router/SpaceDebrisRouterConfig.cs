using UnityEngine;
using SpaceAce.Auxiliary;

namespace SpaceAce.Gameplay.Inventories
{
    [CreateAssetMenu(fileName = "Space debris router", menuName = "Space ace/Configs/Loot/Space debris router")]
    public sealed class SpaceDebrisRouterConfig : InventoryItemConfig
    {
        [SerializeField] private float _spaceDebrisSpawnSpeedup = SpaceDebrisRouter.MinSpaceDebrisSpawnSpeedup;
        [SerializeField] private float _spaceDebrisSpawnSpeedupRandomDeviation = 0f;

        public RangedFloat SpaceDebrisSpawnSpeedup { get; private set; }

        public override void ApplySettings()
        {
            base.ApplySettings();

            SpaceDebrisSpawnSpeedup = new(_spaceDebrisSpawnSpeedup,
                                          _spaceDebrisSpawnSpeedupRandomDeviation,
                                          SpaceDebrisRouter.MinSpaceDebrisSpawnSpeedup,
                                          SpaceDebrisRouter.MaxSpaceDebrisSpawnSpeedup);
        }

        public override InventoryItem GetItem() => new SpaceDebrisRouter(Rarity,
                                                                         Duration.RandomValue,
                                                                         SpaceDebrisSpawnSpeedup.RandomValue);
    }
}