namespace SpaceAce.Gameplay.Movement.EnemyMovement
{
    public sealed class EvadeLeftBoundDown : EnemyMovementState
    {
        public EvadeLeftBoundDown(EnemyMovement owner) : base(owner) { }

        protected override float GetNextHorizontalSpeed() => Owner.NextHorizontalSpeed;

        protected override float GetNextVerticalSpeed() => -1f * Owner.NextVerticalSpeed;
    }
}