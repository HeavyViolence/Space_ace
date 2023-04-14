using SpaceAce.Auxiliary.StateMachines;
using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Movement.EnemyMovement
{
    public abstract class EnemyMovementState : IState, IEquatable<IState>
    {
        private float _currentHorizontalSpeed;
        private float _previousHorizontalSpeed;
        private float _targetHorizontalSpeed;
        private float _horizontalSpeedDuration;
        private float _horizontalSpeedTransitionDuration;
        private float _horizontalSpeedTimer;

        private float _currentVerticalSpeed;
        private float _previousVerticalSpeed;
        private float _targetVerticalSpeed;
        private float _verticalSpeedDuration;
        private float _verticalSpeedTransitionDuration;
        private float _verticalSpeedTimer;

        protected Vector2 Velocity => new(_currentHorizontalSpeed, _currentVerticalSpeed);
        protected EnemyMovement Owner { get; }
        protected bool StateHasJustBegun { get; private set; }

        public EnemyMovementState(EnemyMovement owner)
        {
            Owner = owner;
        }

        #region interfaces

        public override bool Equals(object obj) => Equals(obj as IState);

        public bool Equals(IState other) => other is not null && GetType().Equals(other.GetType());

        public override int GetHashCode() => GetType().GetHashCode();

        public static bool operator ==(EnemyMovementState x, EnemyMovementState y)
        {
            if (x is null)
            {
                if (y is null)
                {
                    return true;
                }

                return false;
            }

            return x.Equals(y);
        }

        public static bool operator !=(EnemyMovementState x, EnemyMovementState y) => !(x == y);

        public void OnStateEnter()
        {
            StateHasJustBegun = true;

            InitializeHorizontalSpeed();
            InitializeVerticalSpeed();
        }

        public void OnStateUpdate()
        {
            UpdateHorizontalSpeed();
            UpdateVerticalSpeed();

            if (StateHasJustBegun)
            {
                StateHasJustBegun = false;
            }
        }

        public void OnStateFixedUpdate()
        {
            Owner.Body.MovePosition(Owner.Body.position + Velocity * Time.fixedDeltaTime);
        }

        public void OnStateExit()
        {
            Owner.PreviousStateExitVelocity = Velocity;
        }

        #endregion

        #region state behavior

        private void InitializeHorizontalSpeed()
        {
            _horizontalSpeedTimer = 0f;
            _previousHorizontalSpeed = Owner.PreviousStateExitVelocity.x;
            _targetHorizontalSpeed = GetNextHorizontalSpeed();
            _horizontalSpeedDuration = Owner.HorizontalSpeedDuration;
            _horizontalSpeedTransitionDuration = Owner.HorizontalSpeedTransitionDuration;
        }

        private void InitializeVerticalSpeed()
        {
            _verticalSpeedTimer = 0f;
            _previousVerticalSpeed = Owner.PreviousStateExitVelocity.y;
            _targetVerticalSpeed = GetNextVerticalSpeed();
            _verticalSpeedDuration = Owner.VerticalSpeedDuration;
            _verticalSpeedTransitionDuration = Owner.VerticalSpeedTransitionDuration;
        }

        private void UpdateHorizontalSpeed()
        {
            _horizontalSpeedTimer += Time.deltaTime;

            if (_horizontalSpeedTimer > _horizontalSpeedDuration + _horizontalSpeedTransitionDuration)
            {
                _horizontalSpeedTimer = 0f;
                _previousHorizontalSpeed = _currentHorizontalSpeed;
                _targetHorizontalSpeed = GetNextHorizontalSpeed();
                _horizontalSpeedDuration = Owner.HorizontalSpeedDuration;
                _horizontalSpeedTransitionDuration = Owner.HorizontalSpeedTransitionDuration;
            }

            _currentHorizontalSpeed = Mathf.Lerp(_previousHorizontalSpeed,
                                                 _targetHorizontalSpeed,
                                                 _horizontalSpeedTimer / _horizontalSpeedTransitionDuration);
        }

        private void UpdateVerticalSpeed()
        {
            _verticalSpeedTimer += Time.deltaTime;

            if (_verticalSpeedTimer > _verticalSpeedDuration + _verticalSpeedTransitionDuration)
            {
                _verticalSpeedTimer = 0f;
                _previousVerticalSpeed = _currentVerticalSpeed;
                _targetVerticalSpeed = GetNextVerticalSpeed();
                _verticalSpeedDuration = Owner.HorizontalSpeedDuration;
                _verticalSpeedTransitionDuration = Owner.VerticalSpeedTransitionDuration;
            }

            _currentVerticalSpeed = Mathf.Lerp(_previousVerticalSpeed,
                                               _targetVerticalSpeed,
                                               _verticalSpeedTimer / _verticalSpeedTransitionDuration);
        }

        protected abstract float GetNextHorizontalSpeed();

        protected abstract float GetNextVerticalSpeed();

        #endregion
    }
}