using System;
using System.Collections.Generic;

namespace AE.Core.StateMachines
{
    public abstract class StateMachine<T> where T : IState
    {
        private T _currentState;

        public T CurrentState => _currentState;
        public event Action<T> OnStateChanged;

        public virtual void ChangeState(T newState)
        {
            if (EqualityComparer<T>.Default.Equals(_currentState, newState))
                return;

            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();
            OnStateChanged?.Invoke(_currentState);
        }

        public void Update()
        {
            _currentState?.Update();
        }
    }
}