using AE.Interactions.Manipulable;
using AE.Interactions.Trigger;
using AE.SimplifyDialogue;
using UnityEngine;

namespace AE.Core.Utility
{
    public class Utilities : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverScreen;
        [field: SerializeField] public CanvasGroup FadeCanvasGroup { get; private set; }
        [field: SerializeField] public SimplifyDialogueView SimplifyDialogueView { get; private set; }
        [field: SerializeField] public Crosshair Crosshair { get; private set; }
        [field: SerializeField] public ManipulationHintUI ManipulationHintUI { get; private set; }
        [field: SerializeField] public Ghost Ghost { get; private set; }

        public void ShowGameOverScreen()
        {
            gameOverScreen.SetActive(true);
        }
    }
}