namespace SpaceAce.Gameplay.Movement.EnemyMovement
{
    public sealed class FlyForward : EnemyMovementState
    {
        public FlyForward(EnemyMovement owner) : base(owner) { }

        protected override float GetNextHorizontalSpeed() => 0f;

        protected override float GetNextVerticalSpeed() => -1f * Owner.Config.VerticalSpeed.RandomValue;
    }
}