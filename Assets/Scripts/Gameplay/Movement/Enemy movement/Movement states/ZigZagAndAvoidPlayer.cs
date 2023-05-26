using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Movement.EnemyMovement
{
    public sealed class ZigZagAndAvoidPlayer : EnemyMovementState
    {
        public ZigZagAndAvoidPlayer(EnemyMovement owner) : base(owner) { }

        protected override float GetNextHorizontalSpeed()
        {
            if (Owner.PreviousStateType.Equals(typeof(FlyForward)))
            {
                return Owner.NextHorizontalSpeed * AuxMath.RandomSign;
            }

            var hit = Physics2D.CircleCast(Owner.Body.position,
                                           Mathf.Infinity,
                                           Vector2.down,
                                           Mathf.Infinity,
                                           LayerMask.GetMask("Player"));

            if (hit.collider != null)
            {
                if (Owner.Body.position.x > hit.transform.position.x)
                {
                    return Owner.NextHorizontalSpeed;
                }

                return -1f * Owner.NextHorizontalSpeed;
            }
            else
            {
                return Owner.NextHorizontalSpeed * AuxMath.RandomSign;
            }
        }

        protected override float GetNextVerticalSpeed()
        {
            if (Owner.PreviousStateType.Equals(typeof(FlyForward)))
            {
                return Owner.NextVerticalSpeed * AuxMath.RandomSign;
            }

            if (StateHasJustBegun)
            {
                if (Owner.PreviousStateExitVelocity.y > 0f)
                {
                    return Owner.NextVerticalSpeed;
                }
                else
                {
                    return -1f * Owner.NextVerticalSpeed;
                }
            }

            if (Owner.Body.velocity.y > 0f)
            {
                return -1f * Owner.NextVerticalSpeed;
            }
            else
            {
                return Owner.NextVerticalSpeed;
            }
        }
    }
}