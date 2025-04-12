using System;
using AE.Interactions.Inspectable;
using AE.Interactions.Manipulable;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace AE.Interactions
{
    [Serializable]
    public class InteractableModule
    {
        [SerializeField] private InspectSystem inspectSystem;
        [SerializeField] private ItemManipulationSystem itemManipulationSystem;
        [SerializeField] private InteractionInitializer interactionInitializer;

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterComponent(inspectSystem);
            builder.RegisterComponent(itemManipulationSystem);

            builder.RegisterComponent(interactionInitializer);
        }
    }
}