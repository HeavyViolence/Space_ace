using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    [CreateAssetMenu(fileName = "Closest damageable entity target supplier",
                     menuName = "Space ace/Configs/Shooting/Target suppliers/Closest damageable entity target supplier")]
    public sealed class ClosestDamageableEntityTargetSupplier : TargetSupplier
    {
        public override Transform GetTarget(Vector2 origin)
        {
            int layerMask = LayerMask.GetMask("Bosses", "Enemies", "Meteors", "Space debris");
            var hit = Physics2D.CircleCast(origin, SearchWidth, Vector2.up, float.PositiveInfinity, layerMask);

            return hit.collider == null ? null : hit.transform;
        }
    }
}