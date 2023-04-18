namespace SpaceAce.Gameplay.Movement.EnemyMovement
{
    public sealed class PirateScoutMovement : EnemyMovement
    {
        private FlyForward _flyForward;

        private ZigZag _zigZag;

        private EvadeLeftBoundUpOrDown _evadeLeftBoundUpOrDown;
        private EvadeRightBoundUpOrDown _evadeRightBoundUpOrDown;

        private EvadeUpperBoundLeftOrRight _evadeUpperBoundLeftOrRight;
        private EvadeLowerBoundLeftOrRight _evadeLowerBoundLeftOrRight;

        protected override void OnSetup()
        {
            base.OnSetup();

            _flyForward = new(this);

            _zigZag = new(this);

            _evadeLeftBoundUpOrDown = new(this);
            _evadeRightBoundUpOrDown = new(this);

            _evadeUpperBoundLeftOrRight = new(this);
            _evadeLowerBoundLeftOrRight = new(this);

            AddTransition(_flyForward, _zigZag, () => Body.position.y < UpperBound);

            AddTransition(_zigZag, _evadeLeftBoundUpOrDown, () => Body.position.x < LeftBound);
            AddTransition(_zigZag, _evadeRightBoundUpOrDown, () => Body.position.x > RightBound);
            AddTransition(_zigZag, _evadeLowerBoundLeftOrRight, () => Body.position.y < LowerBound);
            AddTransition(_zigZag, _evadeUpperBoundLeftOrRight, () => Body.position.y > UpperBound);

            AddTransition(_evadeLeftBoundUpOrDown, _zigZag, () => Body.position.x > LeftBound);
            AddTransition(_evadeLeftBoundUpOrDown, _evadeLowerBoundLeftOrRight, () => Body.position.y < LowerBound);

            AddTransition(_evadeRightBoundUpOrDown, _zigZag, () => Body.position.x < RightBound);
            AddTransition(_evadeRightBoundUpOrDown, _evadeLowerBoundLeftOrRight, () => Body.position.y < LowerBound);

            AddTransition(_evadeUpperBoundLeftOrRight, _zigZag, () => Body.position.y < UpperBound);
            AddTransition(_evadeUpperBoundLeftOrRight, _evadeLeftBoundUpOrDown, () => Body.position.x < LeftBound);
            AddTransition(_evadeUpperBoundLeftOrRight, _evadeRightBoundUpOrDown, () => Body.position.x > RightBound);

            AddTransition(_evadeLowerBoundLeftOrRight, _zigZag, () => Body.position.y > LowerBound); ;
            AddTransition(_evadeLowerBoundLeftOrRight, _evadeLeftBoundUpOrDown, () => Body.position.x < LeftBound);
            AddTransition(_evadeLowerBoundLeftOrRight, _evadeRightBoundUpOrDown, () => Body.position.x > RightBound);

            SetDefaultState(_zigZag);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            SetInitialState(_flyForward);
        }
    }
}