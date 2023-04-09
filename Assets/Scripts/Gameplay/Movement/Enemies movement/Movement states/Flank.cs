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
                    return Owner.Config.HorizontalSpeed.RandomValue;
                }
                else
                {
                    return -1f * Owner.Config.HorizontalSpeed.RandomValue;
                }
            }
            else
            {
                return Owner.Config.HorizontalSpeed.RandomValue * AuxMath.RandomSign;
            }
        }

        protected override float GetNextVerticalSpeed()
        {
            if (Owner.PreviousStateType.Equals(typeof(FlyForward)))
            {
                return Owner.Config.VerticalSpeed.RandomValue * AuxMath.RandomSign * EntryVerticalSpeedFactor;
            }

            if (StateHasJustBegun)
            {
                if (Owner.PreviousStateExitVelocity.y > 0f)
                {
                    return Owner.Config.VerticalSpeed.RandomValue;
                }
                else
                {
                    return -1f * Owner.Config.VerticalSpeed.RandomValue;
                }
            }
            else
            {
                return Owner.Config.VerticalSpeed.RandomValue * AuxMath.RandomSign;
            }
        }
    }
}