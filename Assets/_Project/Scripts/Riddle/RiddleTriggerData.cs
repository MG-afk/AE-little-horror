using UnityEngine;
using UnityEngine.Events;

namespace AE.Riddle
{
    [CreateAssetMenu(fileName = "Riddle Trigger Data", menuName = "Riddle/TriggerData")]
    public sealed class RiddleTriggerData : ScriptableObject
    {
        [field: SerializeField] public string Key { get; private set; }
        [field: SerializeField] public string Value { get; private set; }
    }
}