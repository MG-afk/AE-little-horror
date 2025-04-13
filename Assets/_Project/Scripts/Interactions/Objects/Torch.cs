using AE.Core.Systems.Audio;
using AE.Core.Utility;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace AE.Interactions.Objects
{
    public class Torch : InteractableItem
    {
        private const string FailureText = "Looks like I can't fire that up";

        [SerializeField] private GameObject fire;
        [SerializeField] private AudioSource fireAudio;

        private Utilities _utilities;
        private AudioSystem _audioSystem;

        [Inject]
        private void Construct(
            Utilities utilities,
            AudioSystem audioSystem)
        {
            _utilities = utilities;
            _audioSystem = audioSystem;
        }

        public void SetFireActive(bool active)
        {
            if (!active)
            {
                _audioSystem.PlaySound(SoundType.FireBlownOut);
                _utilities.SimplifyDialogueView.Show(FailureText, 1.5f).Forget();
            }
            else
            {
                _audioSystem.PlaySound(SoundType.FireUp, .5f);
                fireAudio.Play();
            }

            fire.SetActive(active);
        }
    }
}