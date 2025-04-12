using System;
using VContainer;

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