using System;
using AE.Core.Commands;
using AE.Core.Event;
using AE.Core.GlobalGameState;
using AE.Core.Input;
using AE.Core.Systems;
using AE.Core.Systems.Audio;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace AE.Core
{
    [Serializable]
    public class CoreModule
    {
        [SerializeField] private AudioSystem audioSystem;
        [SerializeField] private CameraSystem cameraSystem;

        public void Install(IContainerBuilder builder)
        {
            builder.Register<CommandBus>(Lifetime.Singleton);
            builder.Register<EventManager>(Lifetime.Singleton);

            InstallSystems(builder);
            InstallStates(builder);
        }

        public void InstallSystems(IContainerBuilder builder)
        {
            builder.Register<InputSystem>(Lifetime.Singleton);
            builder.Register<GameTimeSystem>(Lifetime.Singleton);

            builder.RegisterComponent(audioSystem).AsSelf();
            builder.RegisterComponent(cameraSystem).AsSelf();
        }

        public void InstallStates(IContainerBuilder builder)
        {
            builder.Register<GlobalGameStateMachine>(Lifetime.Singleton);

            builder.Register<GameplayState>(Lifetime.Singleton);
            builder.Register<PauseState>(Lifetime.Singleton);
            builder.Register<InspectState>(Lifetime.Singleton);
            builder.Register<GameOverState>(Lifetime.Singleton);
        }
    }
}