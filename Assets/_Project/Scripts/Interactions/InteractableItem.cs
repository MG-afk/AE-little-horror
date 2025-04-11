using UnityEngine;

namespace AE.Interactions
{
    public abstract class InteractableItem : MonoBehaviour, IInteractable
    {
        public Transform Transform => transform;
        public abstract string Text { get; }

        public abstract void Interact();
    }
}