using AE.Core.Systems.Audio;
using AE.Core.Utility;
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

        [Inject]
        private void Construct(Utilities utilities, AudioSystem audioSystem)
        {
            _utilities = utilities;
            _audioSource = audioSystem;
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