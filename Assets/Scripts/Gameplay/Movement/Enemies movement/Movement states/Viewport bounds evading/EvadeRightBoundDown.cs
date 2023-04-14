namespace SpaceAce.Gameplay.Movement.EnemyMovement
{
    public sealed class EvadeRightBoundDown : EnemyMovementState
    {
        public EvadeRightBoundDown(EnemyMovement owner) : base(owner) { }

        protected override float GetNextHorizontalSpeed() => -1f * Owner.HorizontalSpeed;

        protected override float GetNextVerticalSpeed() => -1f * Owner.VerticalSpeed;
    }
}