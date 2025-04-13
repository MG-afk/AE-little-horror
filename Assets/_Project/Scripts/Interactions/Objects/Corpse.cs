using AE.Riddle;
using UnityEngine;

namespace AE.Interactions.Objects
{
    public class Corpse : RiddleItem
    {
        [SerializeField] private Vector3[] positions;

        private void Awake()
        {
            SetPosition(0);
        }

        public void SetPosition(int index)
        {
            transform.localPosition = positions[index];
        }
    }
}