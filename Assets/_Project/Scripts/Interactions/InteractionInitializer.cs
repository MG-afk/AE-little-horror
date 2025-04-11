using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

namespace AE.Interactions
{
    public class InteractionInitializer : MonoBehaviour
    {
        [SerializeField] private List<InteractableItem> interactableItems;

        [Inject]
        private void Construct(IObjectResolver objectResolver)
        {
            foreach (var interactableItem in interactableItems)
            {
                objectResolver.Inject(interactableItem);
            }
        }

#if UNITY_EDITOR

        private void Reset()
        {
            interactableItems =
                FindObjectsByType<InteractableItem>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
        }

#endif
    }
}