using AE.Core.Input;
using AE.Interactions.Manipulable;
using UnityEngine;
using VContainer;

namespace AE.Interactions.Inspectable
{
    public class InspectableItem : InteractableItem
    {
        [SerializeField] private Rigidbody rb;

        private InspectSystem _inspectSystem;
        private ItemManipulationSystem _manipulationSystem;
        private InputSystem _inputSystem;

        public Rigidbody Rigidbody => rb;

        [Inject]
        private void Construct(
            InspectSystem inspectSystem,
            ItemManipulationSystem manipulationSystem,
            InputSystem inputSystem)
        {
            _inspectSystem = inspectSystem;
            _manipulationSystem = manipulationSystem;
            _inputSystem = inputSystem;
        }

        public override void Interact()
        {
            if (_inputSystem.HoldSecondKey && _manipulationSystem.CanPickupItem(this))
            {
                _manipulationSystem.PickupItem(this);
            }
            else
            {
                _inspectSystem.SetItem(this);
            }
        }
    }
}