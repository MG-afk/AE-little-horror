using AE.Core;
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

        [SerializeField] private Utilities utilities;
        [SerializeField] private InteractionInitializer interactionInitializer;

        protected override void Configure(IContainerBuilder builder)
        {
            coreModule.Install(builder);
            pauseModule.Install(builder);
            riddleModule.Install(builder);

            builder.RegisterComponent(utilities);
            builder.RegisterComponent(interactionInitializer);

            builder.RegisterEntryPoint<PauseReactor>();
            builder.RegisterEntryPoint<Bootstrapper>();
        }
    }
}