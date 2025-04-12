using AE.Core.Systems.Audio;
using UnityEngine;
using VContainer;

namespace AE.Interactions.Trigger
{
    public class TriggerAudio : BaseTrigger
    {
        private AudioSystem _audioSystem;

        private bool _wasTriggered;

        [Inject]
        private void Construct(AudioSystem audioSystem)
        {
            _audioSystem = audioSystem;
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (_wasTriggered)
                return;

            if (!other.CompareTag("Player"))
                return;

            _audioSystem.PlaySound(SoundType.DoorKnocking);

            _wasTriggered = true;
        }
    }
}