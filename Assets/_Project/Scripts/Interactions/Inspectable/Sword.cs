using UnityEngine;

namespace AE.Interactions.Inspectable
{
    public class Sword : InspectableItem
    {
        [SerializeField] private GameObject text;

        private void Awake()
        {
            text.gameObject.SetActive(false);
        }
    }
}