using AE.Riddle;
using UnityEngine;
using VContainer;

namespace AE.Interactions.Inspectable
{
    public class Sword : InspectableItem
    {
        [SerializeField] private GameObject _text;

        private RiddleBlackboard _blackboard;

        [Inject]
        private void Construct(RiddleBlackboard blackboard)
        {
            _blackboard = blackboard;

            _blackboard.NewValueSet += OnBlackboardChanged;
        }

        private void OnDestroy()
        {
            _blackboard.NewValueSet -= OnBlackboardChanged;
        }

        private void OnBlackboardChanged(string key, string value)
        {
            if (_blackboard.CheckCondition(Condition))
            {
                _text.gameObject.SetActive(true);
            }
        }
    }
}