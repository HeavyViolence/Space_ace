namespace SpaceAce.Gameplay.Movement.EnemyMovement
{
    public sealed class EvadeLeftBoundUp : EnemyMovementState
    {
        public EvadeLeftBoundUp(EnemyMovement owner) : base(owner) { }

        protected override float GetNextHorizontalSpeed() => Owner.NextHorizontalSpeed;

        protected override float GetNextVerticalSpeed() => Owner.NextVerticalSpeed;
    }
}