using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    [CreateAssetMenu(fileName = "Closest enemy target supplier", menuName = "Space ace/Configs/Shooting/Target suppliers/Closest enemy target supplier")]
    public sealed class ClosestEnemyTargetSupplier : TargetSupplier
    {
        public override Transform GetTarget(Vector2 origin)
        {
            int layerMask = LayerMask.GetMask("Bosses", "Enemies");
            var hit = Physics2D.CircleCast(origin, SearchWidth, Vector2.up, float.PositiveInfinity, layerMask);

            return hit.collider == null ? null : hit.transform;
        }
    }
}