using SpaceAce.Auxiliary;

namespace SpaceAce.Gameplay.Movement.EnemyMovement
{
    public sealed class Flank : EnemyMovementState
    {
        private const float EntryVerticalSpeedFactor = 0.25f;

        public Flank(EnemyMovement owner) : base(owner) { }

        protected override float GetNextHorizontalSpeed()
        {
            if (StateHasJustBegun)
            {
                if (Owner.PreviousStateExitVelocity.x > 0f)
                {
                    return Owner.HorizontalSpeed;
                }
                else
                {
                    return -1f * Owner.HorizontalSpeed;
                }
            }
            else
            {
                return Owner.HorizontalSpeed * AuxMath.RandomSign;
            }
        }

        protected override float GetNextVerticalSpeed()
        {
            if (Owner.PreviousStateType.Equals(typeof(FlyForward)))
            {
                return Owner.VerticalSpeed * AuxMath.RandomSign * EntryVerticalSpeedFactor;
            }

            if (StateHasJustBegun)
            {
                if (Owner.PreviousStateExitVelocity.y > 0f)
                {
                    return Owner.VerticalSpeed;
                }
                else
                {
                    return -1f * Owner.VerticalSpeed;
                }
            }
            else
            {
                return Owner.VerticalSpeed * AuxMath.RandomSign;
            }
        }
    }
}