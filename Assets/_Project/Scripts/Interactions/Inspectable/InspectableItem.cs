using AE.Core.Commands;
using UnityEngine;
using VContainer;

namespace AE.Interactions.Inspectable
{
    public class InspectableItem : InteractableItem
    {
        [SerializeField] private Rigidbody rb;

        private CommandBus _commandBus;

        public Rigidbody Rigidbody => rb;

        [Inject]
        private void Construct(CommandBus commandBus)
        {
            _commandBus = commandBus;
        }

        public override void Interact()
        {
            _commandBus.Execute(new ChangeToManipulationModeCommand(this));
        }
    }
}