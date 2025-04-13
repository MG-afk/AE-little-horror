using AE.Core.Utility;
using AE.Riddle;
using AE.SimplifyDialogue;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace AE.Interactions.Inspectable
{
    public class Skull : InspectableItem
    {
        [SerializeField] private RiddleItemData riddleItemDataToDisplayText;

        private SimplifyDialogueView _simplifyDialogue;
        private RiddleBlackboard _blackboard;

        [Inject]
        private void Construct(Utilities utilities, RiddleBlackboard blackboard)
        {
            _simplifyDialogue = utilities.SimplifyDialogueView;
            _blackboard = blackboard;
        }

        public override void Interact()
        {
            base.Interact();

            if (_blackboard.CheckCondition(riddleItemDataToDisplayText.Condition))
            {
                _simplifyDialogue.Show("I need to bring them somewhere").Forget();
            }
        }
    }
}