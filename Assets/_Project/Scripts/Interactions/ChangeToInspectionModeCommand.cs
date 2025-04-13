using AE.Core.Commands;
using AE.Core.Systems;
using AE.Interactions.Inspectable;
using AE.Interactions.Manipulable;
using VContainer;

namespace AE.Interactions
{
    public class ChangeToInspectionModeCommand : ICommand
    {
        private readonly InspectableItem _inspectableItem;

        private InspectSystem _inspectSystem;

        public ChangeToInspectionModeCommand(InspectableItem inspectableItem)
        {
            _inspectableItem = inspectableItem;
        }

        [Inject]
        private void Construct(InspectSystem inspectSystem)
        {
            _inspectSystem = inspectSystem;
        }

        public void Execute()
        {
            _inspectSystem.SetItem(_inspectableItem);
        }
    }
}