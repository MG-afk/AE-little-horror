using AE.Core.Utility;
using AE.Riddle;
using Cysharp.Threading.Tasks;
using VContainer;

namespace AE.Interactions.Objects
{
    public class TextOnWall : InteractableItem
    {
        private RiddleBlackboard _blackboard;
        private Utilities _utilities;

        [Inject]
        private void Construct(RiddleBlackboard blackboard, Utilities utilities)
        {
            _blackboard = blackboard;
            _utilities = utilities;
        }

        public void Awake()
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
            if (!_blackboard.CheckCondition(Condition))
                return;

            gameObject.SetActive(true);
        }

        public override void Interact()
        {
            _utilities.SimplifyDialogueView.Show("What that means?").Forget();
        }
    }
}