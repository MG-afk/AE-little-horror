using AE.Core.Systems.Audio;
using AE.Core.Utility;
using Cysharp.Threading.Tasks;
using VContainer;

namespace AE.Interactions.Objects
{
    public class Door : InteractableItem
    {
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
            await _utilities.SimplifyDialogueView.Show("Locked. Of course it is.");
        }
    }
}