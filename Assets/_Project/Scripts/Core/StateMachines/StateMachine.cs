using System.Collections.Generic;

namespace AE.Core.StateMachines
{
    public abstract class StateMachine<T> where T : IState
    {
        protected T CurrentState { get; private set; }

        protected void ChangeState(T newState)
        {
            if (EqualityComparer<T>.Default.Equals(CurrentState, newState))
                return;

            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }
    }
}