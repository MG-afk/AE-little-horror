using AE.Riddle;
using UnityEngine;
using VContainer;

namespace AE.Interactions.Trigger
{
    public class BloodZoneTrigger : BaseTrigger
    {
        private const string SkullTag = "Skull";

        private int _counter = 3;
        private RiddleBlackboard _blackboard;

        [Inject]
        private void Construct(RiddleBlackboard blackboard)
        {
            _blackboard = blackboard;
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(SkullTag))
                return;

            _counter--;

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
        }
    }
}