using UnityEngine;

namespace AE.Interactions
{
    public interface IInteractable
    {
        void Interact(Transform interactor);
        Transform GetTransform();
    }
}