using AE.Core;
using AE.Core.Horror;
using AE.Core.Utility;
using AE.Interactions;
using AE.Pause;
using AE.Riddle;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace AE
{
    public sealed class AppLifetimeScope : LifetimeScope
    {
        [SerializeField] private CoreModule coreModule;
        [SerializeField] private PauseModule pauseModule;
        [SerializeField] private RiddleModule riddleModule;
        [SerializeField] private InteractableModule interactableModule;

        [SerializeField] private Utilities utilities;

        protected override void Configure(IContainerBuilder builder)
        {
            coreModule.Install(builder);
            pauseModule.Install(builder);
            riddleModule.Install(builder);
            interactableModule.Install(builder);

            builder.RegisterComponent(utilities);

            builder.RegisterEntryPoint<HorrorAudioService>();
            builder.RegisterEntryPoint<Bootstrapper>();
        }
    }
}