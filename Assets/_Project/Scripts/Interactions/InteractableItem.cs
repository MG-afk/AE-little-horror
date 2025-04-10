using UnityEngine;

namespace AE.Interactions
{
    public class InteractableItem : MonoBehaviour, IInteractable
    {
        [SerializeField] private string interactionMessage = "Item was interacted with!";

        public void Interact()
        {
            Debug.Log($"{interactionMessage} By: {gameObject.name}");
        }
    }
}