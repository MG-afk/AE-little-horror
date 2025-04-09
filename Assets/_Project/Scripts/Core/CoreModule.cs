using System;
using AE.Core.GlobalGameState;
using AE.Core.Systems;
using UnityEngine;
using VContainer;

namespace AE.Core
{
    [Serializable]
    public class CoreModule
    {
        [SerializeField] private AudioSystem audioSystem;
        [SerializeField] private CameraSystem cameraSystem;

        public void Install(IContainerBuilder builder)
        {
            InstallSystems(builder);
            InstallStates(builder);
        }

        public void InstallSystems(IContainerBuilder builder)
        {
            builder.Register<InputSystem>(Lifetime.Singleton);
            builder.Register<GameTimeSystem>(Lifetime.Singleton);

            builder.RegisterInstance(audioSystem);
            builder.RegisterInstance(cameraSystem);
        }

        public void InstallStates(IContainerBuilder builder)
        {
            builder.RegisterInstance(new GameplayState());
            builder.RegisterInstance(new PauseState());
            builder.RegisterInstance(new InspectState());
        }
    }
}