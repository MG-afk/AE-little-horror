﻿namespace AE.Core.StateMachines
{
    public interface IState
    {
        void Enter();
        void Exit();
        void Update();
    }
}