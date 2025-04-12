using System;
using AE.Core.Utility;
using AE.Riddle;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace AE.Interactions.Objects
{
    public class Torch : InteractableItem
    {
        [SerializeField] private string fireId;
        [SerializeField] private GameObject fire;

        private RiddleBlackboard _blackboard;
        private Utilities _utilities;

        [Inject]
        private void Construct(RiddleBlackboard blackboard, Utilities utilities)
        {
            _blackboard = blackboard;
            _utilities = utilities;

            _blackboard.NewValueSet += OnBlackboardChanged;
        }

        private void OnDestroy()
        {
            _blackboard.NewValueSet -= OnBlackboardChanged;
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

                _utilities.SimplifyDialogueView.Show("Looks like I can't fire that up", 1.5f).Forget();

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