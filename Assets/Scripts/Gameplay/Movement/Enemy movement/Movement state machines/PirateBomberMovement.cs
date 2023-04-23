namespace SpaceAce.Gameplay.Movement.EnemyMovement
{
    public sealed class PirateBomberMovement : EnemyMovement
    {
        private FlyForward _flyForward;

        private ZigZagAndSeekPlayer _zigZagAndAvoidPlayer;

        private EvadeLeftBoundUp _evadeLeftBoundUp;
        private EvadeRightBoundUp _evadeRightBoundUp;

        private EvadeUpperBoundLeftOrRight _evadeUpperBoundLeftOrRight;
        private EvadeLowerBoundLeftOrRight _evadeLowerBoundLeftOrRight;

        protected override void OnSetup()
        {
            base.OnSetup();

            _flyForward = new(this);

            _zigZagAndAvoidPlayer = new(this);

            _evadeLeftBoundUp = new(this);
            _evadeRightBoundUp = new(this);

            _evadeUpperBoundLeftOrRight = new(this);
            _evadeLowerBoundLeftOrRight = new(this);

            AddTransition(_flyForward, _zigZagAndAvoidPlayer, () => Body.position.y < UpperBound);

            AddTransition(_zigZagAndAvoidPlayer, _evadeLeftBoundUp, () => Body.position.x < LeftBound);
            AddTransition(_zigZagAndAvoidPlayer, _evadeRightBoundUp, () => Body.position.x > RightBound);
            AddTransition(_zigZagAndAvoidPlayer, _evadeLowerBoundLeftOrRight, () => Body.position.y < LowerBound);
            AddTransition(_zigZagAndAvoidPlayer, _evadeUpperBoundLeftOrRight, () => Body.position.y > UpperBound);

            AddTransition(_evadeLeftBoundUp, _zigZagAndAvoidPlayer, () => Body.position.x > LeftBound);
            AddTransition(_evadeLeftBoundUp, _evadeUpperBoundLeftOrRight, () => Body.position.y > UpperBound);

            AddTransition(_evadeRightBoundUp, _zigZagAndAvoidPlayer, () => Body.position.x < RightBound);
            AddTransition(_evadeRightBoundUp, _evadeUpperBoundLeftOrRight, () => Body.position.y > UpperBound);

            AddTransition(_evadeUpperBoundLeftOrRight, _zigZagAndAvoidPlayer, () => Body.position.y < UpperBound);
            AddTransition(_evadeUpperBoundLeftOrRight, _evadeLeftBoundUp, () => Body.position.x < LeftBound);
            AddTransition(_evadeUpperBoundLeftOrRight, _evadeRightBoundUp, () => Body.position.x > RightBound);

            AddTransition(_evadeLowerBoundLeftOrRight, _zigZagAndAvoidPlayer, () => Body.position.y > LowerBound);
            AddTransition(_evadeLowerBoundLeftOrRight, _evadeLeftBoundUp, () => Body.position.x < LeftBound);
            AddTransition(_evadeLowerBoundLeftOrRight, _evadeRightBoundUp, () => Body.position.x > RightBound);

            SetDefaultState(_zigZagAndAvoidPlayer);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            SetInitialState(_flyForward);
        }
    }
}