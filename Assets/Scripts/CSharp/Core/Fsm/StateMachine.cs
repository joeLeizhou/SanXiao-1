using System.Collections.Generic;
using CSharp.Core.Manager;
using Orcas.Core;

namespace SanXiao.Core
{
    public class StateMachine<T> : IStateMachine
    {
        private T _target;
        private Dictionary<string, StateBase<T>> _states;
        private StateBase<T> _currentState;
        private string _nextState;
        private bool _nextForce;
        private object[] _nextParams;
        public bool Enable;

        internal StateMachine(T target)
        {
            Enable = true;
            _target = target;
            _states = new Dictionary<string, StateBase<T>>();
        }

        public StateMachine<T> AddState(string stateName, StateBase<T> state)
        {
            state.BindTarget(_target);
            _states.Add(stateName, state);
            return this;
        }

        public void ChangeState(string stateName, bool force = false, params object[] parameters)
        {
            if (force == false && _currentState?.Key == stateName) return;
            _currentState?.End();
            _nextState = null;
            _currentState = _states[stateName];
            _currentState.Enter(parameters);
            
        }

        public void ChangeStateDelay(string stateName, bool force = false, params object[] parameters)
        {
            _nextState = stateName;
            _nextForce = force;
            _nextParams = parameters;
        }

        public StateBase<T> GetCurrentState()
        {
            return _currentState;
        }
        
        public void Update()
        {
            if (Enable && _currentState != null)
            {
                _currentState.Update();
            }
        }

        public void UpdateEndOfFrame()
        {
            if (_nextState != null)
            {
                ChangeState(_nextState, _nextForce, _nextParams);
            }
        }

        public void Dispose()
        {
            GameManager.Instance.GetManager<FsmManager>().RemoveStateMachine(this);
        }
    }
}