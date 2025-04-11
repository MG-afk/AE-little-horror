using AE.Riddle;
using UnityEngine;
using VContainer;

namespace AE.Interactions.Objects
{
    public class Torch : InteractableItem
    {
        [SerializeField] private string fireId;
        [SerializeField] private GameObject fire;

        private RiddleBlackboard _blackboard;

        [Inject]
        private void Construct(RiddleBlackboard blackboard)
        {
            _blackboard = blackboard;
        }

        private void Awake()
        {
            _blackboard.NewValueSet += OnBlackboardChanged;
        }

        private void OnBlackboardChanged(string key, string value)
        {
            if (key != RiddleConstant.FireProgress)
                return;

            if (value == fireId)
                return;

            if (value == RiddleConstant.Incorrect)
            {
                SetActiveCandlesticks(false);
                _blackboard.Remove(key);
            }
        }

        public override void Interact()
        {
            if (!_blackboard.CheckCondition(Condition))
            {
                _blackboard.SetValue(RiddleConstant.FireProgress, RiddleConstant.Incorrect);
                return;
            }

            SetActiveCandlesticks(true);

            _blackboard.SetValue(Key, fireId);
        }

        private void SetActiveCandlesticks(bool active)
        {
            fire.SetActive(active);
        }
    }
}