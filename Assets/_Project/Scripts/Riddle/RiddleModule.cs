using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace AE.Riddle
{
    [Serializable]
    public class RiddleModule
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<RiddleBlackboard>(Lifetime.Singleton);
        }
    }
}