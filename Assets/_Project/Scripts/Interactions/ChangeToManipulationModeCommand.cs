using AE.Core.Commands;
using AE.Interactions.Inspectable;
using AE.Interactions.Manipulable;
using VContainer;

namespace AE.Interactions
{
    public class ChangeToManipulationModeCommand : ICommand
    {
        private readonly InspectableItem _inspectableItem;

        private ItemManipulationSystem _manipulationSystem;

        public ChangeToManipulationModeCommand(InspectableItem inspectableItem)
        {
            _inspectableItem = inspectableItem;
        }

        [Inject]
        private void Construct(ItemManipulationSystem manipulationSystem)
        {
            _manipulationSystem = manipulationSystem;
        }

        public void Execute()
        {
            if (_manipulationSystem.CanPickupItem(_inspectableItem))
            {
                _manipulationSystem.PickupItem(_inspectableItem);
            }
        }
    }
}