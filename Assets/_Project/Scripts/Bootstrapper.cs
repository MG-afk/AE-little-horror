using AE.Core;
using AE.Core.Commands;
using AE.Core.GlobalGameState;
using AE.Core.Utility;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using VContainer.Unity;

namespace AE
{
    [UsedImplicitly]
    public sealed class Bootstrapper : IStartable
    {
        private const float FadeInDuration = 1f;

        private readonly Utilities _utilities;
        private readonly CommandBus _commandBus;

        public Bootstrapper(Utilities utilities, CommandBus commandBus)
        {
            _utilities = utilities;
            _commandBus = commandBus;
        }

        public void Start()
        {
            _utilities.FadeOut(FadeInDuration);
            _commandBus.Execute(new ChangeGameStateCommand(GameMode.Gameplay));

            DialogueSequence().Forget();
        }

        private async UniTask DialogueSequence()
        {
            await _utilities.SimplifyDialogueView.Show("Where am I?");
            await _utilities.SimplifyDialogueView.Show("Shit, It is too dark here");
        }
    }
}