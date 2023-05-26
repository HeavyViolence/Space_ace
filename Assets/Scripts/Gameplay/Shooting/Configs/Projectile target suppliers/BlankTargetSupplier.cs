using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    [CreateAssetMenu(fileName = "Blank target supplier", menuName = "Space ace/Configs/Shooting/Target suppliers/Blank target supplier")]
    public sealed class BlankTargetSupplier : TargetSupplier
    {
        public override Transform GetTarget(Vector2 origin) => null;
    }
}