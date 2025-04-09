using JetBrains.Annotations;
using UnityEngine;

namespace AE.Core.Systems
{
    [UsedImplicitly]
    public class AudioSystem : MonoBehaviour
    {
        [SerializeField] private AudioSource gameplayAudio;
        [SerializeField] private AudioSource inspectionAudio;
        [SerializeField] private AudioSource pauseAudio;

        public AudioSystem(AudioSource gameplayAudio, AudioSource inspectionAudio, AudioSource pauseAudio)
        {
            this.gameplayAudio = gameplayAudio;
            this.inspectionAudio = inspectionAudio;
            this.pauseAudio = pauseAudio;
        }

        public void SwitchToGameplayAudio()
        {
            pauseAudio.Stop();
            inspectionAudio.Stop();
            gameplayAudio.Play();
            Debug.Log("Switched to gameplay audio");
        }

        public void SwitchToInspectionAudio()
        {
            gameplayAudio.Stop();
            pauseAudio.Stop();
            inspectionAudio.Play();
            Debug.Log("Switched to inspection audio");
        }

        public void SwitchToPauseAudio()
        {
            gameplayAudio.Stop();
            inspectionAudio.Stop();
            pauseAudio.Play();
            Debug.Log("Switched to pause audio");
        }
    }
}