using AE.Riddle;
using UnityEngine;
using VContainer;

namespace AE.Interactions.Trigger
{
    public class Ghost : BaseTrigger
    {
        private RiddleBlackboard _blackboard;

        [Inject]
        private void Construct(RiddleBlackboard blackboard)
        {
            _blackboard = blackboard;
        }

        private void Awake()
        {
            gameObject.SetActive(false);

            _blackboard.NewValueSet += OnBlackboardChanged;
        }

        private void OnDestroy()
        {
            _blackboard.NewValueSet -= OnBlackboardChanged;
        }

        private void OnBlackboardChanged(string key, string value)
        {
            if (key == RiddleConstant.EndKey && value == RiddleConstant.EndValue)
            {
                gameObject.SetActive(true);
            }
        }


        protected override void OnTriggerEnter(Collider other)
        {
            //Nothing
        }
    }
}