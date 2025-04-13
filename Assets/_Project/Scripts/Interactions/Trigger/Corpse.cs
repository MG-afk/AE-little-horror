using AE.Riddle;
using UnityEngine;
using VContainer;

namespace AE.Interactions.Trigger
{
    public class Corpse : BaseTrigger
    {
        [SerializeField] private Vector3[] positions;

        private RiddleBlackboard _blackboard;

        [Inject]
        private void Construct(RiddleBlackboard blackboard)
        {
            _blackboard = blackboard;
        }

        private void Awake()
        {
            SetPosition(positions[0]);
            _blackboard.NewValueSet += OnBlackboardChanged;
        }

        private void OnDestroy()
        {
            _blackboard.NewValueSet -= OnBlackboardChanged;
        }

        private void OnBlackboardChanged(string key, string value)
        {
            if (key != RiddleConstant.SkullPicking)
                return;

            switch (value)
            {
                case "0":
                    SetPosition(positions[0]);
                    break;
                case "1":
                    SetPosition(positions[1]);
                    break;
                case "2":
                    SetPosition(positions[2]);
                    break;
            }
        }


        protected override void OnTriggerEnter(Collider other)
        {
        }

        private void SetPosition(Vector3 position)
        {
            transform.localPosition = position;
        }
    }
}