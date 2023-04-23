using SpaceAce.Auxiliary;

namespace SpaceAce.Gameplay.Movement.EnemyMovement
{
    public sealed class EvadeRightBoundUpOrDown : EnemyMovementState
    {
        public EvadeRightBoundUpOrDown(EnemyMovement owner) : base(owner) { }

        protected override float GetNextHorizontalSpeed() => -1f * Owner.NextHorizontalSpeed;

        protected override float GetNextVerticalSpeed() => Owner.NextVerticalSpeed * AuxMath.RandomSign;
    }
}