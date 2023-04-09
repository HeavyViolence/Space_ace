using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Auxiliary.StateMachines
{
    public abstract class MonoBehaviourStateMachine : MonoBehaviour
    {
        private readonly Dictionary<Type, List<Transition>> _availableTransitions = new();

        private IState _currentState;
        private List<Transition> _currentStateTransitions = new();

        private IState _defaultState;
        private readonly List<Transition> _defaultStateTransitions = new();

        private IState _previousState;

        public Type CurrentStateType => _currentState.GetType();
        public Type DefaultStateType => _defaultState.GetType();
        public Type PreviousStateType => _previousState.GetType();

        private void Awake() => OnSetup();

        private void OnEnable() => OnInitialize();

        private void Update() => OnUpdate();

        private void FixedUpdate() => _currentState?.OnStateFixedUpdate();

        private void OnUpdate()
        {
            Transition t = GetTransition();

            if (TransitionIsValid(t))
            {
                SetCurrentState(t.To);
            }

            _currentState?.OnStateUpdate();
        }

        private Transition GetTransition()
        {
            foreach (var t in _currentStateTransitions)
            {
                if (t.Condition())
                {
                    return t;
                }
            }

            foreach (var t in _defaultStateTransitions)
            {
                if (t.Condition())
                {
                    return t;
                }
            }

            return null;
        }

        private bool TransitionIsValid(Transition t) => t is not null && _currentState.GetType().Equals(t.To.GetType()) == false;

        private void SetCurrentState(IState state)
        {
            _currentState.OnStateExit();

            _previousState = _currentState;
            _currentState = state;
            _currentStateTransitions = _availableTransitions[state.GetType()];

            _currentState.OnStateEnter();
        }

        protected void SetDefaultState(IState state) => _defaultState = state;

        protected void SetInitialState(IState state)
        {
            _currentState = state;
            _currentStateTransitions = _availableTransitions[state.GetType()];
        }

        protected void AddTransition(IState from, IState to, Func<bool> condition)
        {
            Transition t = new(to, condition);

            if (_availableTransitions.TryGetValue(from.GetType(), out var transitions))
            {
                if (transitions.Contains(t))
                {
                    throw new ArgumentException("Attempted to add already existing transition!");
                }
                else
                {
                    transitions.Add(t);
                }
            }
            else
            {
                _availableTransitions.Add(from.GetType(), new List<Transition>() { t });
            }
        }

        protected void AddDefaultStateTransition(IState to, Func<bool> condition)
        {
            Transition t = new(to, condition);

            if (_defaultStateTransitions.Contains(t))
            {
                throw new ArgumentException("Attempted to add already existing transition!");
            }
            else
            {
                _defaultStateTransitions.Add(t);
            }
        }

        protected abstract void OnSetup();

        protected abstract void OnInitialize();
    }
}