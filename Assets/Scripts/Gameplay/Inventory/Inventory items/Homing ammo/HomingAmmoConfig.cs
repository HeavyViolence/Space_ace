using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    [CreateAssetMenu(fileName = "Homing ammo", menuName = "Space ace/Configs/Loot/Homing ammo")]
    public sealed class HomingAmmoConfig : InventoryItemConfig
    {
        [SerializeField] private float _homingSpeed = HomingAmmo.MinHomingSpeed;
        [SerializeField] private float _homingSpeedRandomDeviation = 0f;

        public RangedFloat HomingSpeed { get; private set; }

        public override void ApplySettings()
        {
            HomingSpeed = new(_homingSpeed, _homingSpeedRandomDeviation, HomingAmmo.MinHomingSpeed, HomingAmmo.MaxHomingSpeed);

            base.ApplySettings();
        }

        public override InventoryItem GetItem() => new HomingAmmo(Rarity, Duration.RandomValue, HomingSpeed.RandomValue);
    }
}