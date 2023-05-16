namespace SpaceAce.Gameplay.Movement.EnemyMovement
{
    public sealed class EvadeUpperRightCornerDiagonally : EnemyMovementState
    {
        public EvadeUpperRightCornerDiagonally(EnemyMovement owner) : base(owner) { }

        protected override float GetNextHorizontalSpeed() => -1f * Owner.NextHorizontalSpeed;

        protected override float GetNextVerticalSpeed() => -1f * Owner.NextVerticalSpeed;
    }
}