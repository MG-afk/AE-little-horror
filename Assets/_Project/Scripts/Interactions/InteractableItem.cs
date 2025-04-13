using AE.Riddle;
using UnityEngine;

namespace AE.Interactions
{
    public abstract class InteractableItem : RiddleItem, IInteractable
    {
        public Transform Transform => transform;

        public virtual void Interact()
        {
        }
    }
}