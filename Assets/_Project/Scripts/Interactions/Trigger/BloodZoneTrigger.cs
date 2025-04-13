using AE.Core.Utility;
using AE.Riddle;
using AE.SimplifyDialogue;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace AE.Interactions.Trigger
{
    public class BloodZoneTrigger : BaseTrigger
    {
        private const string SkullTag = "Skull";
        private const int CountOfItems = 3;

        private int _counter = CountOfItems;
        private RiddleBlackboard _blackboard;
        private SimplifyDialogueView _simplifyDialogue;

        [Inject]
        private void Construct(
            RiddleBlackboard blackboard,
            Utilities utilities)
        {
            _blackboard = blackboard;
            _simplifyDialogue = utilities.SimplifyDialogueView;
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && _counter == CountOfItems)
            {
                if (_blackboard.HasKey(RiddleConstant.SkullPicking))
                {
                    _simplifyDialogue.Show("I should bring all skulls here").Forget();
                }
                else
                {
                    _simplifyDialogue.Show("Is that a blood?").Forget();
                    _blackboard.SetValue(RiddleConstant.SkullPicking, RiddleConstant.Accept);
                }
            }

            if (!other.CompareTag(SkullTag))
                return;

            _counter--;
            _blackboard.SetValue(RiddleConstant.SkullPicking, _counter.ToString());

            if (_counter <= 0)
            {
                _blackboard.SetValue(RiddleConstant.EndKey, RiddleConstant.EndValue);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(SkullTag))
                return;

            _counter++;
            _blackboard.SetValue(RiddleConstant.SkullPicking, _counter.ToString());
        }
    }
}