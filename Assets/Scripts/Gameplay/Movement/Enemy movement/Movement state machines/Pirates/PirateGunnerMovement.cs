namespace SpaceAce.Gameplay.Movement.EnemyMovement
{
    public sealed class PirateGunnerMovement : EnemyMovement
    {
        private FlyForward _flyForward;

        private ZigZagAroundViewportCenter _zigZagAroundViewportCenter;

        private EvadeLeftBoundUpOrDown _evadeLeftBoundUpOrDown;
        private EvadeRightBoundUpOrDown _evadeRightBoundUpOrDown;

        private EvadeUpperBoundLeftOrRight _evadeUpperBoundLeftOrRight;
        private EvadeLowerBoundLeftOrRight _evadeLowerBoundLeftOrRight;

        protected override void OnSetup()
        {
            base.OnSetup();

            _flyForward = new(this);

            _zigZagAroundViewportCenter = new(this);

            _evadeLeftBoundUpOrDown = new(this);
            _evadeRightBoundUpOrDown = new(this);

            _evadeUpperBoundLeftOrRight = new(this);
            _evadeLowerBoundLeftOrRight = new(this);

            AddTransition(_flyForward, _zigZagAroundViewportCenter, () => Body.position.y < UpperBound);

            AddTransition(_zigZagAroundViewportCenter, _evadeLeftBoundUpOrDown, () => Body.position.x < LeftBound);
            AddTransition(_zigZagAroundViewportCenter, _evadeRightBoundUpOrDown, () => Body.position.x > RightBound);
            AddTransition(_zigZagAroundViewportCenter, _evadeLowerBoundLeftOrRight, () => Body.position.y < LowerBound);
            AddTransition(_zigZagAroundViewportCenter, _evadeUpperBoundLeftOrRight, () => Body.position.y > UpperBound);

            AddTransition(_evadeLeftBoundUpOrDown, _zigZagAroundViewportCenter, () => Body.position.x > LeftBound);
            AddTransition(_evadeLeftBoundUpOrDown, _evadeLowerBoundLeftOrRight, () => Body.position.y < LowerBound);
            AddTransition(_evadeLeftBoundUpOrDown, _evadeUpperBoundLeftOrRight, () => Body.position.y > UpperBound);

            AddTransition(_evadeRightBoundUpOrDown, _zigZagAroundViewportCenter, () => Body.position.x < RightBound);
            AddTransition(_evadeRightBoundUpOrDown, _evadeLowerBoundLeftOrRight, () => Body.position.y < LowerBound);
            AddTransition(_evadeRightBoundUpOrDown, _evadeUpperBoundLeftOrRight, () => Body.position.y > UpperBound);

            AddTransition(_evadeUpperBoundLeftOrRight, _zigZagAroundViewportCenter, () => Body.position.y < UpperBound);
            AddTransition(_evadeUpperBoundLeftOrRight, _evadeLeftBoundUpOrDown, () => Body.position.x < LeftBound);
            AddTransition(_evadeUpperBoundLeftOrRight, _evadeRightBoundUpOrDown, () => Body.position.x > RightBound);

            AddTransition(_evadeLowerBoundLeftOrRight, _zigZagAroundViewportCenter, () => Body.position.y > LowerBound); ;
            AddTransition(_evadeLowerBoundLeftOrRight, _evadeLeftBoundUpOrDown, () => Body.position.x < LeftBound);
            AddTransition(_evadeLowerBoundLeftOrRight, _evadeRightBoundUpOrDown, () => Body.position.x > RightBound);

            SetDefaultState(_zigZagAroundViewportCenter);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            SetInitialState(_flyForward);
        }
    }
}