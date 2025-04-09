using AE.Core;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace AE
{
    public sealed class AppLifetimeScope : LifetimeScope
    {
        [SerializeField] private CoreModule coreModule;
        [SerializeField] private Utilities utilities;

        protected override void Configure(IContainerBuilder builder)
        {
            coreModule.Install(builder);

            builder.RegisterInstance(utilities).As<Utilities>();

            builder.RegisterEntryPoint<Bootstrapper>();
        }
    }
}