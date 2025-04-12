using AE.Interactions;
using UnityEngine;

namespace AE.Core.Utility
{
    public static class Raycaster
    {
        public static bool RaycastCenter(
            this Camera camera,
            float distance,
            LayerMask layerMask,
            out IInteractable interactable)
        {
            var ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            interactable = null;

            if (!Physics.Raycast(ray, out var hit, distance, layerMask))
                return false;

            if (!hit.collider.TryGetComponent(out IInteractable hitInteractable))
                return false;

            interactable = hitInteractable;
            return true;
        }
    }
}