using System;
using AE.Core;
using AE.Core.Commands;
using AE.Core.GlobalGameState;
using AE.Core.Input;
using JetBrains.Annotations;
using VContainer.Unity;

namespace AE.Pause
{
    [UsedImplicitly]
    public class PauseReactor : IInitializable, IDisposable
    {
        private readonly CommandBus _commandBus;
        private readonly InputSystem _inputSystem;

        public PauseReactor(
            CommandBus commandBus,
            InputSystem inputSystem)
        {
            _commandBus = commandBus;
            _inputSystem = inputSystem;
        }

        public void Initialize()
        {
            _inputSystem.Exited += Pause;
        }

        public void Dispose()
        {
            _inputSystem.Exited -= Pause;
        }

        private void Pause()
        {
            _commandBus.Execute(new ChangeGameStateCommand(GameMode.Pause));
        }
    }
}