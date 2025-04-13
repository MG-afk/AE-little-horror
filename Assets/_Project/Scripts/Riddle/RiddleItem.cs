using System.Collections.Generic;
using UnityEngine;

namespace AE.Riddle
{
    public abstract class RiddleItem : MonoBehaviour, IRiddleItem
    {
        [SerializeField] private RiddleItemData riddleData;
        [SerializeField] private List<RiggleTrigger> riggleTriggers;
        [SerializeField] private List<RiddleInteractedEvent> onInteracted;

        public string Key => riddleData?.Key;
        public string Condition => riddleData?.Condition;
        public string Result => riddleData?.Result;
        public IList<RiggleTrigger> Triggers => riggleTriggers;
        public IList<RiddleInteractedEvent> InteractedEvents => onInteracted;
    }
}