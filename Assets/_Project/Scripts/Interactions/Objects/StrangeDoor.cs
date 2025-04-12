using AE.Core.Systems.Audio;
using AE.Core.Utility;
using AE.Riddle;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace AE.Interactions.Objects
{
    public class StrangeDoor : InteractableItem
    {
        [SerializeField] private GameObject bloodOnFloor;

        private Utilities _utilities;
        private AudioSystem _audioSource;
        private RiddleBlackboard _blackboard;

        [Inject]
        private void Construct(Utilities utilities, AudioSystem audioSystem, RiddleBlackboard blackboard)
        {
            _utilities = utilities;
            _audioSource = audioSystem;
            _blackboard = blackboard;

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

            bloodOnFloor.SetActive(true);
        }

        public override void Interact()
        {
            Sequence().Forget();
        }

        private async UniTask Sequence()
        {
            _audioSource.PlaySound(SoundType.DoorClose, transform.position);
            await _utilities.SimplifyDialogueView.Show(
                "There's... something behind this door.\nI can feel it watching me.\n");
        }
    }
}