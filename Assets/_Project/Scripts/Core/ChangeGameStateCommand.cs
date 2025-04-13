using AE.Core.Commands;
using AE.Core.GlobalGameState;
using JetBrains.Annotations;
using VContainer;

namespace AE.Core
{
    [UsedImplicitly]
    public class ChangeGameStateCommand : ICommand
    {
        private readonly GameMode _targetGameMode;

        private IGlobalGameStateMachine _stateMachine;

        public ChangeGameStateCommand(GameMode targetGameMode)
        {
            _targetGameMode = targetGameMode;
        }

        [Inject]
        public void Construct(IGlobalGameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Execute()
        {
            _stateMachine.ChangeState(_targetGameMode);
        }
    }
}