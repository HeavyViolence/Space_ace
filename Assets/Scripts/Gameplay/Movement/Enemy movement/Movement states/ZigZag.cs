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
            else
            {
                if (Owner.Body.velocity.x > 0f)
                {
                    return -1f * Owner.NextHorizontalSpeed;
                }
                else
                {
                    return Owner.NextHorizontalSpeed;
                }
            }
        }

        protected override float GetNextVerticalSpeed()
        {
            if (Owner.PreviousStateType.Equals(typeof(FlyForward)))
            {
                return Owner.NextVerticalSpeed * AuxMath.RandomSign * EntrySpeedFactor;
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
    }
}