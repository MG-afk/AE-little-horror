using AE.Core.Utility;
using AE.Riddle;
using AE.SimplifyDialogue;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace AE.Interactions.Trigger
{
    //TODO: Make it generic
    public class DeliveryZone : BaseTrigger
    {
        private const string PlayerTriggerText = "Maybe I should bring all skulls here";

        [SerializeField] private string itemTag = "Skull";
        [SerializeField] private int countOfItemsToBring = 3;
        [SerializeField] private RiddleTriggerData startTrigger;
        [SerializeField] private RiddleTriggerData thresholdChangedTrigger;
        [SerializeField] private RiddleTriggerData completeTrigger;

        private int _counter;
        private RiddleBlackboard _blackboard;
        private SimplifyDialogueView _simplifyDialogue;
        private bool _textWasDisplayed;

        private void Awake()
        {
            _counter = countOfItemsToBring;
        }

        [Inject]
        private void Construct(
            RiddleBlackboard blackboard,
            Utilities utilities)
        {
            _blackboard = blackboard;
            _simplifyDialogue = utilities.SimplifyDialogueView;

            _blackboard.DataChanged += OnDataChanged;
        }

        private void OnDataChanged(string key, string value)
        {
            if (startTrigger.Key != key || startTrigger.Value != value)
                return;

            gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            _blackboard.DataChanged -= OnDataChanged;
        }


        protected override void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && _counter == countOfItemsToBring)
            {
                if (_textWasDisplayed)
                    return;

                _textWasDisplayed = true;
                _simplifyDialogue.Show(PlayerTriggerText).Forget();
            }

            if (!other.CompareTag(itemTag))
                return;

            _counter--;
            _blackboard.SetValue(thresholdChangedTrigger.Key, _counter.ToString());

            if (_counter <= 0)
            {
                _blackboard.SetValue(completeTrigger.Key, completeTrigger.Value);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(itemTag))
                return;

            _counter++;
            _blackboard.SetValue(thresholdChangedTrigger.Key, _counter.ToString());
        }
    }
}