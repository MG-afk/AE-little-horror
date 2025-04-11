using UnityEngine;

namespace AE.Interactions
{
    public interface IInteractable
    {
        Transform Transform { get; }
        string Condition { get; }
        string Key { get; }

        void Interact();
    }
}