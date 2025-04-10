using JetBrains.Annotations;
using VContainer;

namespace AE.Core.Commands
{
    [UsedImplicitly]
    public class CommandBus
    {
        private readonly IObjectResolver _container;

        [Inject]
        public CommandBus(IObjectResolver container)
        {
            _container = container;
        }

        private ICommand Dispatch(ICommand command)
        {
            _container.Inject(command);
            return command;
        }

        public void Execute(ICommand command)
        {
            var executable = Dispatch(command);
            executable.Execute();
        }
    }
}