using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace AE.Riddle
{
    [Serializable]
    public class RiddleInteractedEvent
    {
        [SerializeField] private RiddleSystem.Result resultOfInteraction;
        [SerializeField] private UnityEvent onInteracted;
        [Header("Optional")] [SerializeField] private RiddleTriggerData triggerData;

        public RiddleSystem.Result ResultOfInteraction => resultOfInteraction;
        public UnityEvent OnInteracted => onInteracted;
        public RiddleTriggerData TriggerData => triggerData;
    }
}