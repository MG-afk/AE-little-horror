using AE.Core;
using AE.Core.Utility;
using AE.Pause;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace AE
{
    public sealed class AppLifetimeScope : LifetimeScope
    {
        [SerializeField] private CoreModule coreModule;
        [SerializeField] private Utilities utilities;
        [SerializeField] private PauseModule pauseModule;

        protected override void Configure(IContainerBuilder builder)
        {
            coreModule.Install(builder);
            pauseModule.Install(builder);

            builder.RegisterComponent(utilities);

            builder.RegisterEntryPoint<PauseReactor>();
            builder.RegisterEntryPoint<Bootstrapper>();
        }
    }
}