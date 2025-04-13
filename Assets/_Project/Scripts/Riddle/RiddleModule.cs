using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace AE.Riddle
{
    [Serializable]
    public class RiddleModule
    {
        [SerializeField] private List<RiddleItem> items;

        public void Install(IContainerBuilder builder)
        {
            items = Object.FindObjectsByType<RiddleItem>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .ToList();

            builder.Register<RiddleSystem>(Lifetime.Singleton).WithParameter(items);
            builder.Register<RiddleBlackboard>(Lifetime.Singleton);
        }
    }
}