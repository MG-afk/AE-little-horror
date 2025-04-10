using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace AE.Pause
{
    [Serializable]
    public class PauseModule
    {
        [SerializeField] private PauseView pauseView;

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterComponent(pauseView);
        }
    }
}