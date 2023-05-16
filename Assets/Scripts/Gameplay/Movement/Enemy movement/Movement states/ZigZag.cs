using SpaceAce.Auxiliary;

namespace SpaceAce.Gameplay.Movement.EnemyMovement
{
    public sealed class ZigZag : EnemyMovementState
    {
        public ZigZag(EnemyMovement owner) : base(owner) { }

        protected override float GetNextHorizontalSpeed()
        {
            if (StateHasJustBegun)
            {
                if (Owner.PreviousStateExitVelocity.x > 0f)
                {
                    return Owner.NextHorizontalSpeed;
                }
                else
                {
                    return -1f * Owner.NextHorizontalSpeed;
                }
            }

            if (Owner.Body.velocity.x > 0f)
            {
                return -1f * Owner.NextHorizontalSpeed;
            }
            else
            {
                return Owner.NextHorizontalSpeed;
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