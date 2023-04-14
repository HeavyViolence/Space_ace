using SpaceAce.Auxiliary;

namespace SpaceAce.Gameplay.Movement.EnemyMovement
{
    public sealed class EvadeUpperBoundLeftOrRight : EnemyMovementState
    {
        public EvadeUpperBoundLeftOrRight(EnemyMovement owner) : base(owner) { }

        protected override float GetNextHorizontalSpeed() => Owner.HorizontalSpeed * AuxMath.RandomSign;

        protected override float GetNextVerticalSpeed() => -1f * Owner.VerticalSpeed;
    }
}