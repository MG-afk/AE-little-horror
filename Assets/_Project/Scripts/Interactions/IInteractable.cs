using UnityEngine;

namespace AE.Interactions
{
    public interface IInteractable
    {
        Transform Transform { get; }

        void Interact();
    }
}