using SpaceAce.Gameplay.Inventories;
using SpaceAce.Gameplay.Players;
using System;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Gameplay.Movement
{
    public sealed class PlayerShipMovement : CollidableMovement, IMovementController,
                                                                 IReactiveArmorUser,
                                                                 INanofuelUser
    {
        private const float SpawnPositionDisplacementFactor = 0.5f;

        private Vector2 _speed2D;

        private Coroutine _reactiveArmorRoutine = null;
        private float _reactiveArmorSlowdownFactor = 1f;

        private Coroutine _nanofuelRoutine = null;
        private float _nanofuelSpeedup = 0f;

        protected override void OnEnable()
        {
            base.OnEnable();

            transform.position = new(0f, Config.LowerBound * SpawnPositionDisplacementFactor, 0f);
            _speed2D = new(Config.HorizontalSpeed.RandomValue, Config.VerticalSpeed.RandomValue);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_reactiveArmorRoutine != null)
            {
                StopCoroutine(_reactiveArmorRoutine);
                _reactiveArmorRoutine = null;
                _reactiveArmorSlowdownFactor = 1f;
            }

            if (_nanofuelRoutine != null)
            {
                StopCoroutine(_nanofuelRoutine);
                _nanofuelRoutine = null;
                _nanofuelSpeedup = 0f;
            }
        }

        public void Move(Vector2 direction)
        {
            Vector2 clampedDirection = ClampMovementDirection(direction);

            Vector2 velocity = _reactiveArmorSlowdownFactor * Time.fixedDeltaTime * _speed2D * clampedDirection;
            if (_nanofuelSpeedup > 0f) velocity.x += _nanofuelSpeedup;

            Body.MovePosition(Body.position + velocity);
        }

        private Vector2 ClampMovementDirection(Vector2 rawMovementDirection)
        {
            float x = rawMovementDirection.x;
            float y = rawMovementDirection.y;

            if (Body.position.x < Config.LeftBound) x = Mathf.Clamp(x, 0f, 1f);
            if (Body.position.x > Config.RightBound) x = Mathf.Clamp(x, -1f, 0f);
            if (Body.position.y < Config.LowerBound) y = Mathf.Clamp(y, 0f, 1f);
            if (Body.position.y > Config.UpperBound) y = Mathf.Clamp(y, -1f, 0f);

            return new Vector2(x, y);
        }

        public bool Use(ReactiveArmor armor)
        {
            if (armor is null) throw new ArgumentNullException(nameof(armor));

            if (_reactiveArmorRoutine == null)
            {
                _reactiveArmorRoutine = StartCoroutine(ApplyReactiveArmor(armor));
                return true;
            }

            return false;
        }

        private IEnumerator ApplyReactiveArmor(ReactiveArmor armor)
        {
            float timer = 0f;
            _reactiveArmorSlowdownFactor = armor.MovementSlowdown;

            while (timer < armor.Duration)
            {
                timer += Time.deltaTime;

                yield return null;
                while (GamePauser.Access.Paused == true) yield return null;
            }

            _reactiveArmorRoutine = null;
            _reactiveArmorSlowdownFactor = 1f;
        }

        public bool Use(Nanofuel fuel)
        {
            if (fuel is null) throw new ArgumentNullException(nameof(fuel));

            if (_nanofuelRoutine == null)
            {
                _nanofuelRoutine = StartCoroutine(ApplyNanofuel(fuel));
                return true;
            }

            return false;
        }

        private IEnumerator ApplyNanofuel(Nanofuel fuel)
        {
            float timer = 0f;
            _nanofuelSpeedup = fuel.SpeedIncrease;

            while (timer < fuel.Duration)
            {
                timer += Time.deltaTime;

                yield return null;
                while (GamePauser.Access.Paused == true) yield return null;
            }

            _nanofuelRoutine = null;
            _nanofuelSpeedup = 0f;
        }
    }
}