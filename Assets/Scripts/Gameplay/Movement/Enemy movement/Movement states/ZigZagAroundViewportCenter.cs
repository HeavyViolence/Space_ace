using SpaceAce.Auxiliary;

namespace SpaceAce.Gameplay.Movement.EnemyMovement
{
    public sealed class ZigZagAroundViewportCenter : EnemyMovementState
    {
        public ZigZagAroundViewportCenter(EnemyMovement owner) : base(owner) { }

        protected override float GetNextHorizontalSpeed() => Owner.Body.position.x > 0f ? -1f * Owner.NextHorizontalSpeed : Owner.NextHorizontalSpeed;

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

            return Owner.NextVerticalSpeed * AuxMath.RandomSign;
        }
    }
}