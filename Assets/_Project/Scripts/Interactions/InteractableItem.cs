using UnityEngine;

namespace AE.Interactions
{
    public abstract class InteractableItem : MonoBehaviour, IInteractable
    {
        [SerializeField] private string key;
        [SerializeField] private string condition;

        public Transform Transform => transform;

        public string Condition => condition;
        public string Key => key;

        public abstract void Interact();
    }
}