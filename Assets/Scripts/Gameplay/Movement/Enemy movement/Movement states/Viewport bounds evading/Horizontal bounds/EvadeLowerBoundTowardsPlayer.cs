using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Movement.EnemyMovement
{
    public sealed class EvadeLowerBoundTowardsPlayer : EnemyMovementState
    {
        public EvadeLowerBoundTowardsPlayer(EnemyMovement owner) : base(owner) { }

        protected override float GetNextHorizontalSpeed()
        {
            var hit = Physics2D.CircleCast(Owner.Body.position,
                                           Mathf.Infinity,
                                           Vector2.down,
                                           Mathf.Infinity,
                                           LayerMask.GetMask("Player"));

            if (hit.collider != null)
            {
                if (Owner.Body.position.x > hit.collider.transform.position.x)
                {
                    return -1f * Owner.NextHorizontalSpeed;
                }

                return Owner.NextHorizontalSpeed;
            }
            else
            {
                return Owner.NextHorizontalSpeed * AuxMath.RandomSign;
            }
        }

        protected override float GetNextVerticalSpeed() => Owner.NextVerticalSpeed;
    }
}