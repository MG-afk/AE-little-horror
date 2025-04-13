using UnityEngine;

namespace AE.Riddle
{
    [CreateAssetMenu(fileName = "Riddle Item Data", menuName = "Riddle/ItemData")]
    public sealed class RiddleItemData : ScriptableObject
    {
        [field: SerializeField] public string Key { get; private set; }
        [field: SerializeField] public string Condition { get; private set; }
        [field: SerializeField] public string Result { get; private set; }
    }
}