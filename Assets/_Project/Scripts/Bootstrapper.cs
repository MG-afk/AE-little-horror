using System;
using AE.Core;
using AE.Core.Commands;
using AE.Core.GlobalGameState;
using AE.Core.Types;
using AE.Core.Utility;
using JetBrains.Annotations;
using VContainer.Unity;

namespace AE
{
    [UsedImplicitly]
    public sealed class Bootstrapper : IStartable, IDisposable
    {
        private readonly Utilities _utilities;
        private readonly CommandBus _commandBus;

        public Bootstrapper(
            Utilities utilities,
            CommandBus commandBus)
        {
            _utilities = utilities;
            _commandBus = commandBus;
        }

        public void Start()
        {
            _utilities.FadeOut(1f);
            _commandBus.Execute(new ChangeGameStateCommand(GameMode.Gameplay));
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }
    }
}