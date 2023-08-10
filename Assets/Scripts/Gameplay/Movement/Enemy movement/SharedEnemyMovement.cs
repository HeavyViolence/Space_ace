using SpaceAce.Gameplay.Amplifications;
using SpaceAce.Gameplay.Inventories;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Gameplay.Movement.EnemyMovement
{
    public abstract class SharedEnemyMovement : EnemyMovement, IAmplifiable,
                                                               IStasisFieldUser
    {
        private const float BoundsNarrowingFactor = 0.85f;

        private float _amplificationFactor = 1f;

        private float _stasisFieldSlowdown = 1f;
        private Coroutine _stasisFieldRoutine = null;

        public override float NextHorizontalSpeed => Config.HorizontalSpeed.RandomValue *
                                                     _amplificationFactor *
                                                     _stasisFieldSlowdown;

        public override float NextHorizontalSpeedDuration => Config.HorizontalSpeedDuration.RandomValue / _amplificationFactor;

        public override float NextHorizontalSpeedTransitionDuration => Config.HorizontalSpeedTransitionDuration.RandomValue /
                                                                       _amplificationFactor *
                                                                       _stasisFieldSlowdown;

        public override float NextVerticalSpeed => Config.VerticalSpeed.RandomValue *
                                                   _amplificationFactor *
                                                   _stasisFieldSlowdown;

        public override float NextVerticalSpeedDuration => Config.VerticalSpeedDuration.RandomValue / _amplificationFactor;

        public override float NextVerticalSpeedTransitionDuration => Config.VerticalSpeedTransitionDuration.RandomValue /
                                                                     _amplificationFactor *
                                                                     _stasisFieldSlowdown;

        public override float NextCollisionDamage => Config.CollisionDamageEnabled ? Config.CollisionDamage.RandomValue * _amplificationFactor : 0f;

        public override float LeftBound => _amplificationFactor == 1f ? Config.LeftBound : Config.LeftBound * BoundsNarrowingFactor;
        public override float RightBound => _amplificationFactor == 1f ? Config.RightBound : Config.RightBound * BoundsNarrowingFactor;
        public override float UpperBound => _amplificationFactor == 1f ? Config.UpperBound : Config.UpperBound * BoundsNarrowingFactor;
        public override float LowerBound => Config.LowerBound;

        protected override void OnDeinitialize()
        {
            base.OnDeinitialize();

            _amplificationFactor = 1f;

            if (_stasisFieldRoutine != null)
            {
                StopCoroutine(_stasisFieldRoutine);
                _stasisFieldRoutine = null;
            }
        }

        public void Amplify(float factor) => _amplificationFactor = factor;

        public bool Use(StasisField field)
        {
            if (_stasisFieldRoutine == null)
            {
                _stasisFieldRoutine = StartCoroutine(StasisFieldRoutine(field));
                return true;
            }

            return false;
        }

        private IEnumerator StasisFieldRoutine(StasisField field)
        {
            _stasisFieldSlowdown = 1f - field.Slowdown;
            float timer = 0f;

            while (timer < field.Duration)
            {
                timer += Time.deltaTime;

                yield return null;
                while (GamePauser.Access.Paused == true) yield return null;
            }

            _stasisFieldSlowdown = 1f;
            _stasisFieldRoutine = null;
        }
    }
}