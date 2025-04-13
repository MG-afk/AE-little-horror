using System;
using System.Collections.Generic;
using AE.Interactions;
using JetBrains.Annotations;

namespace AE.Riddle
{
    [UsedImplicitly]
    public class RiddleSystem : IDisposable
    {
        public enum Result
        {
            Success,
            Failure,
        }

        private readonly RiddleBlackboard _blackboard;
        private readonly List<RiddleItem> _riddleRiddleItems;
        private readonly HashSet<string> _currentlyProcessingKeys = new();

        public RiddleSystem(
            RiddleBlackboard blackboard,
            List<RiddleItem> riddleItems)
        {
            _blackboard = blackboard;
            _riddleRiddleItems = riddleItems;

            _blackboard.DataChanged += OnDataChanged;
        }

        public void Interacted(IInteractable interactable)
        {
            var riddleItem = interactable as RiddleItem;

            var result = Interact(riddleItem);

            if (riddleItem == null)
                return;

            foreach (var interactedEvent in riddleItem.InteractedEvents)
            {
                if (interactedEvent.ResultOfInteraction != result)
                    continue;

                interactedEvent.OnInteracted?.Invoke();

                if (interactedEvent.TriggerData == null)
                    continue;

                _blackboard.SetValue(interactedEvent.TriggerData.Key, interactedEvent.TriggerData.Value);
            }
        }

        private Result Interact(RiddleItem riddleItem)
        {
            if (riddleItem == null)
                return Result.Failure;

            if (!_blackboard.CheckCondition(riddleItem.Condition))
                return Result.Failure;

            return _blackboard.TrySetResult(riddleItem.Result) ? Result.Success : Result.Failure;
        }

        private void OnDataChanged(string key, string value)
        {
            if (!_currentlyProcessingKeys.Add(key))
                return;

            //Just I want to make sure that no one will create the Stackoverflow :3 
            try
            {
                foreach (var riddleItem in _riddleRiddleItems)
                {
                    foreach (var trigger in riddleItem.Triggers)
                    {
                        if (trigger.Key != key)
                            continue;

                        if (trigger.Value != value)
                            continue;

                        trigger.OnTrigger?.Invoke();
                    }
                }
            }
            finally
            {
                _currentlyProcessingKeys.Remove(key);
            }
        }

        public void Dispose()
        {
            _blackboard.DataChanged -= OnDataChanged;
        }
    }
}