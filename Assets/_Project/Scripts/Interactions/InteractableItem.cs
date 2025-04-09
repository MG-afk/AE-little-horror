using UnityEngine;

namespace AE.Interactions
{
    public class InteractableItem : MonoBehaviour, IInteractable
    {
        [SerializeField] private string interactionMessage = "Item was interacted with!";
        
        public void Interact(Transform interactor)
        {
            Debug.Log($"{interactionMessage} By: {interactor.name}");
            // Add your interaction logic here
        }
        
        public Transform GetTransform()
        {
            return transform;
        }
    }
}