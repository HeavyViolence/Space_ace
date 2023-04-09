namespace SpaceAce.Gameplay.Movement.EnemyMovement
{
    public sealed class FlankingEnemyMovement : EnemyMovement
    {
        private FlyForward _flyForward;

        private Flank _flank;

        private EvadeLeftBoundDown _evadeLeftBoundDown;
        private EvadeRightBoundDown _evadeRightBoundDown;

        private EvadeUpperBoundLeftOrRight _evadeUpperBoundLeftOrRight;
        private EvadeLowerBoundLeftOrRight _evadeLowerBoundLeftOrRight;

        protected override void OnSetup()
        {
            base.OnSetup();

            _flyForward = new(this);

            _flank = new(this);

            _evadeLeftBoundDown = new(this);
            _evadeRightBoundDown = new(this);

            _evadeUpperBoundLeftOrRight = new(this);
            _evadeLowerBoundLeftOrRight = new(this);

            AddTransition(_flyForward, _flank, () => Body.position.y < Config.UpperBound);

            AddTransition(_flank, _evadeLeftBoundDown, () => Body.position.x < Config.LeftBound);
            AddTransition(_flank, _evadeRightBoundDown, () => Body.position.x > Config.RightBound);
            AddTransition(_flank, _evadeLowerBoundLeftOrRight, () => Body.position.y < Config.LowerBound);
            AddTransition(_flank, _evadeUpperBoundLeftOrRight, () => Body.position.y > Config.UpperBound);

            AddTransition(_evadeLeftBoundDown, _flank, () => Body.position.x > Config.LeftBound);
            AddTransition(_evadeLeftBoundDown, _evadeLowerBoundLeftOrRight, () => Body.position.y < Config.LowerBound);

            AddTransition(_evadeRightBoundDown, _flank, () => Body.position.x < Config.RightBound);
            AddTransition(_evadeRightBoundDown, _evadeLowerBoundLeftOrRight, () => Body.position.y < Config.LowerBound);

            AddTransition(_evadeUpperBoundLeftOrRight, _flank, () => Body.position.y < Config.UpperBound);
            AddTransition(_evadeUpperBoundLeftOrRight, _evadeLeftBoundDown, () => Body.position.x < Config.LeftBound);
            AddTransition(_evadeUpperBoundLeftOrRight, _evadeRightBoundDown, () => Body.position.x > Config.RightBound);

            AddTransition(_evadeLowerBoundLeftOrRight, _flank, () => Body.position.y > Config.LowerBound); ;
            AddTransition(_evadeLowerBoundLeftOrRight, _evadeLeftBoundDown, () => Body.position.x < Config.LeftBound);
            AddTransition(_evadeLowerBoundLeftOrRight, _evadeRightBoundDown, () => Body.position.x > Config.RightBound);

            SetDefaultState(_flank);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            SetInitialState(_flyForward);
        }
    }
}