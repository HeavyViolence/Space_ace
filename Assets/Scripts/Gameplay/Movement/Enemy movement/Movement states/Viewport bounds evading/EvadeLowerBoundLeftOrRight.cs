using SpaceAce.Auxiliary;

namespace SpaceAce.Gameplay.Movement.EnemyMovement
{
    public sealed class EvadeLowerBoundLeftOrRight : EnemyMovementState
    {
        public EvadeLowerBoundLeftOrRight(EnemyMovement owner) : base(owner) { }

        protected override float GetNextHorizontalSpeed() => Owner.NextHorizontalSpeed * AuxMath.RandomSign;

        protected override float GetNextVerticalSpeed() => Owner.NextVerticalSpeed;
    }
}