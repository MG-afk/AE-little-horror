using System;
using UnityEngine;
using UnityEngine.Events;

namespace AE.Riddle
{
    [Serializable]
    public class RiggleTrigger
    {
        [SerializeField] private RiddleTriggerData triggerData;
        [SerializeField] private UnityEvent onTrigger;

        public string Key => triggerData.Key;
        public string Value => triggerData.Value;

        public UnityEvent OnTrigger => onTrigger;
    }
}