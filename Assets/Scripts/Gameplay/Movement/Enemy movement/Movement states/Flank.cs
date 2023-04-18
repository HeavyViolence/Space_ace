using SpaceAce.Auxiliary;

namespace SpaceAce.Gameplay.Movement.EnemyMovement
{
    public sealed class Flank : EnemyMovementState
    {
        public Flank(EnemyMovement owner) : base(owner) { }

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
                return Owner.NextHorizontalSpeed * AuxMath.RandomSign;
            }
        }

        protected override float GetNextVerticalSpeed()
        {
            if (Owner.PreviousStateType.Equals(typeof(FlyForward)))
            {
                return Owner.NextVerticalSpeed * AuxMath.RandomSign * EntrySpeedFactor;
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
            else
            {
                return Owner.NextVerticalSpeed * AuxMath.RandomSign;
            }
        }
    }
}