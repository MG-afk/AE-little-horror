using AE.Interactions.Manipulable;
using AE.Interactions.Trigger;
using AE.SimplifyDialogue;
using UnityEngine;
using UnityEngine.UI;

namespace AE.Core.Utility
{
    //TODO: That class is a helper to speed prototyping it should be refactored later
    public class Utilities : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverScreen;
        [field: SerializeField] public CanvasGroup FadeCanvasGroup { get; private set; }
        [field: SerializeField] public SimplifyDialogueView SimplifyDialogueView { get; private set; }
        [field: SerializeField] public Crosshair Crosshair { get; private set; }
        [field: SerializeField] public ManipulationHintUI ManipulationHintUI { get; private set; }
        [field: SerializeField] public Ghost Ghost { get; private set; }
        [field: SerializeField] public GameObject PlayerGameObject { get; private set; }

        public void ShowGameOverScreen()
        {
            gameOverScreen.SetActive(true);

            //TODO: Create GameOver Screen
            gameOverScreen.GetComponentInChildren<Button>().onClick.AddListener(ExitGame);
        }

        public static void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}