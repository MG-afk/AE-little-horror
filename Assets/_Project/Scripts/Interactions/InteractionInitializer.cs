using System.Collections.Generic;
using System.Linq;
using AE.Interactions.Trigger;
using UnityEngine;
using VContainer;

namespace AE.Interactions
{
    public class InteractionInitializer : MonoBehaviour
    {
        [SerializeField] private List<InteractableItem> interactableItems;
        [SerializeField] private List<BaseTrigger> triggers;

        [Inject]
        private void Construct(IObjectResolver objectResolver)
        {
            foreach (var interactableItem in interactableItems)
            {
                objectResolver.Inject(interactableItem);
            }

            foreach (var trigger in triggers)
            {
                objectResolver.Inject(trigger);
            }
        }

#if UNITY_EDITOR

        private void Reset()
        {
            const FindObjectsInactive findObjects = FindObjectsInactive.Exclude;
            const FindObjectsSortMode sortMode = FindObjectsSortMode.None;

            interactableItems = FindObjectsByType<InteractableItem>(findObjects, sortMode).ToList();
            triggers = FindObjectsByType<BaseTrigger>(findObjects, sortMode).ToList();
        }

#endif
    }
}