using System.Collections.Generic;

namespace AE.Core.StateMachines
{
    public abstract class StateMachine<T> where T : IState
    {
        private T _currentState;

        protected void ChangeState(T newState)
        {
            if (EqualityComparer<T>.Default.Equals(_currentState, newState))
                return;

            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();
        }

        public void Update()
        {
            _currentState?.Update();
        }
    }
}