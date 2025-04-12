using AE.Core;
using AE.Core.Utility;
using AE.Interactions;
using AE.Interactions.Inspectable;
using AE.Pause;
using AE.Riddle;
using UnityEngine;
using UnityEngine.Serialization;
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

            builder.RegisterEntryPoint<Bootstrapper>();
        }
    }
}