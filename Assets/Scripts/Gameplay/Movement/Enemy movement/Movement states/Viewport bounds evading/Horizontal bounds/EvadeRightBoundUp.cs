namespace SpaceAce.Gameplay.Movement.EnemyMovement
{
    public sealed class EvadeRightBoundUp : EnemyMovementState
    {
        public EvadeRightBoundUp(EnemyMovement owner) : base(owner) { }

        protected override float GetNextHorizontalSpeed() => -1f * Owner.NextHorizontalSpeed;

        protected override float GetNextVerticalSpeed() => Owner.NextVerticalSpeed;
    }
}