using AE.Core.Systems.Audio;
using AE.SimplifyDialogue;
using UnityEngine;
using VContainer;

namespace AE.Core.Utility
{
    public class Utilities : MonoBehaviour
    {
        private AudioSystem _audioSystem;
        [field: SerializeField] public CanvasGroup FadeCanvasGroup { get; private set; }
        [field: SerializeField] public SimplifyDialogueView SimplifyDialogueView { get; private set; }

        [Inject]
        public void Construct(AudioSystem audioSystem)
        {
            _audioSystem = audioSystem;
        }
    }
}