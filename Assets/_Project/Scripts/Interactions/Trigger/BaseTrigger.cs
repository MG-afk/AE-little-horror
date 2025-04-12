using UnityEngine;

namespace AE.Interactions.Trigger
{
    public abstract class BaseTrigger : MonoBehaviour
    {
        protected abstract void OnTriggerEnter(Collider other);
    }
}