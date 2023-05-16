namespace SpaceAce.Gameplay.Movement.EnemyMovement
{
    public sealed class PirateBossMovement : EnemyMovement
    {
        private FlyForward _flyForward;

        private FlankAndSeekPlayer _flankAndSeekPlayer;

        private EvadeLeftBoundUpOrDown _evadeLeftBoundUpOrDown;
        private EvadeRightBoundUpOrDown _evadeRightBoundUpOrDown;

        private EvadeUpperBoundTowardsPlayer _evadeUpperBoundTowardsPlayer;
        private EvadeLowerBoundTowardsPlayer _evadeLowerBoundTowardsPlayer;

        protected override void OnSetup()
        {
            base.OnSetup();

            _flyForward = new(this);

            _flankAndSeekPlayer = new(this);

            _evadeLeftBoundUpOrDown = new(this);
            _evadeRightBoundUpOrDown = new(this);

            _evadeUpperBoundTowardsPlayer = new(this);
            _evadeLowerBoundTowardsPlayer = new(this);

            AddTransition(_flyForward, _flankAndSeekPlayer, () => Body.position.y < UpperBound);

            AddTransition(_flankAndSeekPlayer, _evadeLeftBoundUpOrDown, () => Body.position.x < LeftBound);
            AddTransition(_flankAndSeekPlayer, _evadeRightBoundUpOrDown, () => Body.position.x > RightBound);
            AddTransition(_flankAndSeekPlayer, _evadeLowerBoundTowardsPlayer, () => Body.position.y < LowerBound);
            AddTransition(_flankAndSeekPlayer, _evadeUpperBoundTowardsPlayer, () => Body.position.y > UpperBound);

            AddTransition(_evadeLeftBoundUpOrDown, _flankAndSeekPlayer, () => Body.position.x > LeftBound);
            AddTransition(_evadeLeftBoundUpOrDown, _evadeLowerBoundTowardsPlayer, () => Body.position.y < LowerBound);
            AddTransition(_evadeLeftBoundUpOrDown, _evadeUpperBoundTowardsPlayer, () => Body.position.y > UpperBound);

            AddTransition(_evadeRightBoundUpOrDown, _flankAndSeekPlayer, () => Body.position.x < RightBound);
            AddTransition(_evadeRightBoundUpOrDown, _evadeLowerBoundTowardsPlayer, () => Body.position.y < LowerBound);
            AddTransition(_evadeRightBoundUpOrDown, _evadeUpperBoundTowardsPlayer, () => Body.position.y > UpperBound);

            AddTransition(_evadeUpperBoundTowardsPlayer, _flankAndSeekPlayer, () => Body.position.y < UpperBound);
            AddTransition(_evadeUpperBoundTowardsPlayer, _evadeLeftBoundUpOrDown, () => Body.position.x < LeftBound);
            AddTransition(_evadeUpperBoundTowardsPlayer, _evadeRightBoundUpOrDown, () => Body.position.x > RightBound);

            AddTransition(_evadeLowerBoundTowardsPlayer, _flankAndSeekPlayer, () => Body.position.y > LowerBound); ;
            AddTransition(_evadeLowerBoundTowardsPlayer, _evadeLeftBoundUpOrDown, () => Body.position.x < LeftBound);
            AddTransition(_evadeLowerBoundTowardsPlayer, _evadeRightBoundUpOrDown, () => Body.position.x > RightBound);

            SetDefaultState(_flankAndSeekPlayer);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            SetInitialState(_flyForward);
        }
    }
}