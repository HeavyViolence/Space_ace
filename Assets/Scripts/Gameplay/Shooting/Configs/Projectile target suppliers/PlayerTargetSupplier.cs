using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    [CreateAssetMenu(fileName = "Player target supplier",
                     menuName = "Space ace/Configs/Shooting/Target suppliers/Player target supplier")]
    public sealed class PlayerTargetSupplier : TargetSupplier
    {
        public override Transform GetTarget(Vector2 origin)
        {
            int layerMask = LayerMask.GetMask("Player");
            var hit = Physics2D.CircleCast(origin, SearchWidth, Vector2.down, float.PositiveInfinity, layerMask);

            return hit.collider == null ? null : hit.transform;
        }
    }
}