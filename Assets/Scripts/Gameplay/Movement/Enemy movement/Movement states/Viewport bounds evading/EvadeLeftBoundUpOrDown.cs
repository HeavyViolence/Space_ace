using SpaceAce.Auxiliary;

namespace SpaceAce.Gameplay.Movement.EnemyMovement
{
    public sealed class EvadeLeftBoundUpOrDown : EnemyMovementState
    {
        public EvadeLeftBoundUpOrDown(EnemyMovement owner) : base(owner) { }

        protected override float GetNextHorizontalSpeed() => Owner.NextHorizontalSpeed;

        protected override float GetNextVerticalSpeed() => Owner.NextVerticalSpeed * AuxMath.RandomSign;
    }
}