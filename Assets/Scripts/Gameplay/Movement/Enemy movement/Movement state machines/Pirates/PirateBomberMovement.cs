namespace SpaceAce.Gameplay.Movement.EnemyMovement
{
    public sealed class PirateBomberMovement : EnemyMovement
    {
        private FlyForward _flyForward;

        private ZigZagAndSeekPlayer _zigZagAndSeekPlayer;

        private EvadeLeftBoundUpOrDown _evadeLeftBoundUpOrDown;
        private EvadeRightBoundUpOrDown _evadeRightBoundUpOrDown;

        private EvadeUpperBoundLeftOrRight _evadeUpperBoundLeftOrRight;
        private EvadeLowerBoundLeftOrRight _evadeLowerBoundLeftOrRight;

        protected override void OnSetup()
        {
            base.OnSetup();

            _flyForward = new(this);

            _zigZagAndSeekPlayer = new(this);

            _evadeLeftBoundUpOrDown = new(this);
            _evadeRightBoundUpOrDown = new(this);

            _evadeUpperBoundLeftOrRight = new(this);
            _evadeLowerBoundLeftOrRight = new(this);

            AddTransition(_flyForward, _zigZagAndSeekPlayer, () => Body.position.y < UpperBound);

            AddTransition(_zigZagAndSeekPlayer, _evadeLeftBoundUpOrDown, () => Body.position.x < LeftBound);
            AddTransition(_zigZagAndSeekPlayer, _evadeRightBoundUpOrDown, () => Body.position.x > RightBound);
            AddTransition(_zigZagAndSeekPlayer, _evadeLowerBoundLeftOrRight, () => Body.position.y < LowerBound);
            AddTransition(_zigZagAndSeekPlayer, _evadeUpperBoundLeftOrRight, () => Body.position.y > UpperBound);

            AddTransition(_evadeLeftBoundUpOrDown, _zigZagAndSeekPlayer, () => Body.position.x > LeftBound);
            AddTransition(_evadeLeftBoundUpOrDown, _evadeUpperBoundLeftOrRight, () => Body.position.y > UpperBound);
            AddTransition(_evadeLeftBoundUpOrDown, _evadeLowerBoundLeftOrRight, () => Body.position.y < LowerBound);

            AddTransition(_evadeRightBoundUpOrDown, _zigZagAndSeekPlayer, () => Body.position.x < RightBound);
            AddTransition(_evadeRightBoundUpOrDown, _evadeUpperBoundLeftOrRight, () => Body.position.y > UpperBound);
            AddTransition(_evadeRightBoundUpOrDown, _evadeLowerBoundLeftOrRight, () => Body.position.y < LowerBound);

            AddTransition(_evadeUpperBoundLeftOrRight, _zigZagAndSeekPlayer, () => Body.position.y < UpperBound);
            AddTransition(_evadeUpperBoundLeftOrRight, _evadeLeftBoundUpOrDown, () => Body.position.x < LeftBound);
            AddTransition(_evadeUpperBoundLeftOrRight, _evadeRightBoundUpOrDown, () => Body.position.x > RightBound);

            AddTransition(_evadeLowerBoundLeftOrRight, _zigZagAndSeekPlayer, () => Body.position.y > LowerBound);
            AddTransition(_evadeLowerBoundLeftOrRight, _evadeLeftBoundUpOrDown, () => Body.position.x < LeftBound);
            AddTransition(_evadeLowerBoundLeftOrRight, _evadeRightBoundUpOrDown, () => Body.position.x > RightBound);

            SetDefaultState(_zigZagAndSeekPlayer);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            SetInitialState(_flyForward);
        }
    }
}