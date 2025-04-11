using UnityEngine;

namespace AE.Interactions
{
    public interface IInteractable
    {
        Transform Transform { get; }
        string Text { get; }
        void Interact();
    }
}